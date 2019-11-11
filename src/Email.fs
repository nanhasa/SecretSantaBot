module Email

open System
open System.Net
open System.Net.Mail
open Domain

let secureString (str : string) = NetworkCredential("", str).SecurePassword

let sendEmail (SenderEmailAddress senderEmail) (password : Security.SecureString) (RecipientEmailAddress recipientEmail) (Subject subject) (Body body) =
    async {
        let sender = MailAddress(senderEmail)
        let receiver = MailAddress(recipientEmail)
        use message = new MailMessage(sender, receiver)
        message.Subject <- subject
        message.Body <- body
        message.IsBodyHtml <- true

        use smtp = new SmtpClient()
        smtp.Host <- "smtp.gmail.com"
        smtp.Port <- 587
        smtp.EnableSsl <- true
        smtp.DeliveryMethod <- SmtpDeliveryMethod.Network
        smtp.Credentials <- NetworkCredential(senderEmail, password)
        do! smtp.SendMailAsync message |> Async.AwaitTask
    }

let sendSecretSantaEmail email = async {
    do! sendEmail email.sender.email email.sender.password email.recipient.email email.subject email.body
}