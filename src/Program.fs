// Learn more about F# at http://fsharp.org
open System
open FSharp.Data
open Argu
open Domain
open Email

// Participants file schema provider
type Participants = CsvProvider<"example-participants.csv", Separators=";">

// CLI arguments provider
type CLIArguments =
    | [<Mandatory; Unique; AltCommandLine("-p")>] Path of Participants_Path : string
    | [<Mandatory; Unique; NoCommandLine>] Sender_Email of email : string
    | [<Mandatory; Unique; NoCommandLine>] Sender_Email_Password of password : string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Path _ -> "Specify the path to the participants csv file"
            | Sender_Email _ -> "Specify the email sender email address in appsettings"
            | Sender_Email_Password _ -> "Specify the email sender email password in appsettings"

let shuffle = Array.sortBy (fun _ -> Guid.NewGuid())

[<EntryPoint>]
let main argv = 
    async {
        try
            // Parse command line arguments and app.config values
            let arguments = 
                ArgumentParser
                    .Create<CLIArguments>(programName = "SecretSanta.exe")
                    .Parse(inputs = argv, configurationReader = ConfigurationReader.FromAppSettingsFile("app.config"), raiseOnUsage = true)

            // Form sender data based on app.config
            let senderEmail = arguments.GetResult Sender_Email
            let password = arguments.GetResult Sender_Email_Password |> secureString
            let sender = createSender (SenderEmailAddress senderEmail) password

            // Parse participants
            let participants = 
                arguments.GetResult Path
                |> Participants.AsyncLoad
                |> Async.RunSynchronously
                |> fun csv -> csv.Rows
                |> Seq.map (fun participant -> createRecipient (RecipientName participant.Name) (RecipientEmailAddress participant.Email))
                |> Seq.toArray

            // Shuffle participants, form pairs and send email
            let circularNextReceiver i = participants.[(i + 1) % participants.Length]
            let! _ = 
                participants
                |> shuffle
                |> Array.mapi (secretSanta participants)
                |> Array.map (createEmail sender >> sendSecretSantaEmail) // TODO: Create some retrying logic
                |> Async.Parallel
            
            printfn "All emails sent to participants, merry christmas!"
        with
        | ex -> printfn "Error: %s" ex.Message
    } 
    |> Async.RunSynchronously
    |> ignore

    0 // return an integer exit code
