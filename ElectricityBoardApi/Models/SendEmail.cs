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