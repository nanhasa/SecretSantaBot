module Domain

open System

type SenderEmailAddress = SenderEmailAddress of string
type RecipientName = RecipientName of string
type RecipientEmailAddress = RecipientEmailAddress of string

type Sender =
    { email : SenderEmailAddress
      password : Security.SecureString }

type Recipient =
    { name : RecipientName
      email : RecipientEmailAddress }

type SecretSantaParticipant = SecretSantaParticipant of string

type SecretSanta =
    { santa : Recipient
      receiver : SecretSantaParticipant }

type Subject = Subject of string
type Body = Body of string

type Email =
    { sender : Sender
      recipient : Recipient
      subject : Subject
      body : Body }

let createSender email password =
    { email = email
      password = password }

let createRecipient name email =
    { name = name
      email = email }

let secretSantaParticipant recipient = 
    let unwrap (RecipientName name) = name
    recipient.name |> unwrap |> SecretSantaParticipant

let secretSanta (participants : Recipient []) i recipient =
    let length = participants.Length
    let circularNextReceiver i = participants.[(i + 1) % length]
    { santa = recipient; receiver = circularNextReceiver i |> secretSantaParticipant }

let createEmail sender secretSanta =
    let santa (RecipientName name) = name
    let recipient (SecretSantaParticipant name) = name
    { sender = sender
      recipient = secretSanta.santa
      subject = Subject "Your secret santa recipient"
      body = Body (sprintf """<pre>
                                  ________________________________________
                                 /                                        \
             ______________      | Dear %-34s|
            /         \    \     |                                        |
           /           \    \    | Your secret santa gift recipient is    |
          / _________   \    |   | %-39s|
          \/     _   \  /    |   |                                        |
           \ /    \   \/_    |   | Please remember this is a secret,      |
           |  O_O_/   || \_  \   | so hus hus... ;)                       |
          / __(_ __   ||   \ /   |                                        |
         /\/___\___\_/  \  /_\   | Merry Christmas!                       |
        /      __        \/   \  | Your Secret Santa Bot                  |
        |                |\___/ /________________________________________/
  ______|________        |  \  
 /              /\       /   \ 
 |              | \     /     \
 \______________\  |___/       \
  _|___            |_____\      \
 /|  __|           |_|| o \      \
| |    \           |_||____\_____/\
\_|____/     /     |        \_____/____
   |        //     |         /     \   \
   |       //      |_________\_/\   \   \__  ______
   |      /        |         /_ |_/_/      \/_//__/|
   |               |________// \|  \      //_//__/||\_
   |               |        \__//   |    / | ||__|/_  \
   |               |        |       |    \_|_/         \
   |  ___________  | |      |       /                  |
   \               \|______|      /              _____ |
    \               \      |      |      \\     _/   / |
     \               \_____|______\___    \    -/___/  |
      \                         ______\_____          /
       \                       / \          \        /
        \__________________    \ |           |______/
                           \    /            /
                            \__/____________/</pre>"""
                (santa secretSanta.santa.name + ",") (recipient secretSanta.receiver + "!")) }