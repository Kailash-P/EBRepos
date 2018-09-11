using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ElectricityBoardApi.Models
{
    public static class SendEmail
    {
        public static void TriggerMail(EmailParameters parameters)
        {
            Execute(parameters);
        }
        static void Execute(EmailParameters parameters)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(parameters.fromEmailAddress, "Admin"),
                Subject = "Consumer Complaint No: " + parameters.ID,
                PlainTextContent = parameters.resolvedMessage
            };
            msg.AddTo(new EmailAddress(parameters.toEmailAddress, parameters.consumerName));
            var response = client.SendEmailAsync(msg);
        }
    }
}