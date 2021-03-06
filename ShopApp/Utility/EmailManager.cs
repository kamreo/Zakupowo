﻿using MimeKit;
using System;
using System.Web;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailKit;
using MailKit.Security;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using ShopApp.Models;
using System.Linq;

namespace ShopApp.Utility
{
    public class EmailManager
    {
        public enum EmailType
        {
            Registration,
            ChangePassword,
            TransactionRequest
        }

        // Return true if email was successfuly sent to receiver
        public static async Task<bool> SendEmailAsync(EmailType emailType, string receiverFirstName, string receiverLastName, string receiverEmail, string optionalPassword = "password")
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));
            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            switch (emailType)
            {
                case EmailType.Registration:
                    builder.HtmlBody = RegistrationText(EncryptEmail(receiverEmail));
                    message.Subject = @"Successful registration!";
                    break;

                case EmailType.ChangePassword:
                    builder.HtmlBody = ChangePasswordText(EncryptEmail(receiverEmail), optionalPassword);
                    message.Subject = @"Your password has been changed.";
                    break;

            }
            builder.HtmlBody += EndOfEmail();

            message.Body = builder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    await client.AuthenticateAsync("zakupowo2020", "Zakupowo2020$$$");

                    await client.SendAsync(message);
                    client.Disconnect(true);
                    result = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public static bool SendEmail(EmailType emailType, string receiverFirstName, string receiverLastName, string receiverEmail, string optionalPassword = "password")
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));

            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            switch (emailType)
            {
                case EmailType.Registration:
                    builder.HtmlBody = RegistrationText(EncryptEmail(receiverEmail));
                    message.Subject = @"Successful registration!";
                    break;

                case EmailType.ChangePassword:
                    builder.HtmlBody = ChangePasswordText(EncryptEmail(receiverEmail), optionalPassword);
                    message.Subject = @"Your password has been changed.";
                    break;
            }
            builder.HtmlBody += EndOfEmail();

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    client.Authenticate("zakupowo2020", "Zakupowo2020$$$");

                    client.Send(message);
                    client.Disconnect(true);
                    result = true;
                    Debug.WriteLine("EMAIL SENT!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public static async Task<bool> SendEmailAsync(EmailType emailType, string receiverFirstName, string receiverLastName, string receiverEmail, string SenderLogin, string SenderFirstName, string SenderLastName, List<BucketItem> PurchaseList, string Message, ShippingAdress Address)
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));
            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            builder.HtmlBody = TransactionRequestText(SenderLogin, SenderFirstName, SenderLastName, PurchaseList, Message, Address);
            message.Subject = @"Purchase request on Zakupowo.";
           
            builder.HtmlBody += EndOfEmail();
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    await client.AuthenticateAsync("zakupowo2020", "Zakupowo2020$$$");

                    await client.SendAsync(message);
                    client.Disconnect(true);
                    result = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public static bool SendEmail(EmailType emailType, string receiverFirstName, string receiverLastName, string receiverEmail, string SenderLogin, string SenderFirstName, string SenderLastName, List<BucketItem> PurchaseList, string Message, ShippingAdress Address)
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));

            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            builder.HtmlBody = TransactionRequestText(SenderLogin, SenderFirstName, SenderLastName, PurchaseList, Message, Address);
            message.Subject = @"Purchase request on Zakupowo.";

            builder.HtmlBody += EndOfEmail();

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    client.Authenticate("zakupowo2020", "Zakupowo2020$$$");

                    client.Send(message);
                    client.Disconnect(true);
                    result = true;
                    Debug.WriteLine("EMAIL SENT!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public static async Task<bool> SendEmailAsync(string receiverFirstName, string receiverLastName, string receiverEmail, string PasswordResetCode)
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));
            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            builder.HtmlBody = PasswordResetText(PasswordResetCode);
            message.Subject = @"Password reset code on zakupowo.";

            builder.HtmlBody += EndOfEmail();
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    await client.AuthenticateAsync("zakupowo2020", "Zakupowo2020$$$");

                    await client.SendAsync(message);
                    client.Disconnect(true);
                    result = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        public static bool SendEmail(string receiverFirstName, string receiverLastName, string receiverEmail, string PasswordResetCode)
        {
            bool result = false;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zakupowo Team", "zakupowo2020@gmail.com"));
            message.To.Add(new MailboxAddress(receiverFirstName + " " + receiverLastName, receiverEmail));

            message.Subject = string.Empty;
            var builder = new BodyBuilder();
            string messageBody = string.Empty;

            builder.HtmlBody = PasswordResetText(PasswordResetCode);
            message.Subject = @"Password reset code on zakupowo.";

            builder.HtmlBody += EndOfEmail();

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                    client.Authenticate("zakupowo2020", "Zakupowo2020$$$");

                    client.Send(message);
                    client.Disconnect(true);
                    result = true;
                    Debug.WriteLine("EMAIL SENT!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return result;
        }

        // A message sent to new registered accounts.
        private static string RegistrationText(string encryptedEmail)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<pre>Thank you for registration!");
            message.AppendLine("");
            message.AppendLine("Now you can:");
            message.AppendLine("    -Browse offers");
            message.AppendLine("    -Change your data");
            message.AppendLine("");
            message.AppendLine("If you want more functionality:");
            message.AppendLine("    -Buy offers");
            message.AppendLine("    -Make offers");
            message.AppendLine("");
            message.AppendLine("<a href=\"http://zakupowo.azurewebsites.net/User/ConfirmRegistration?email=" + encryptedEmail + "\">Click here</a> to fully register your account!<pre>");

            return message.ToString();
        }

        // A message sent when a password was changed.
        private static string ChangePasswordText(string encryptedEmail, string encryptedNewPassword)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(@"<pre>Have you just tried to change your password?");
            message.AppendLine("<a href=\"http://zakupowo.azurewebsites.net/UserPanel/ConfirmPasswordChange?email=" + encryptedEmail + "&psw=" + encryptedNewPassword + "\">Click here</a> to confirm the change.");
            message.AppendLine();
            message.AppendLine();
            message.AppendLine("If you don't recognize this activity, please <a href=\"http://zakupowo.azurewebsites.net/Home/Contact\">contact us</a>.<pre>");

            return message.ToString();
        }

        // A message sent when user makes a purchase.
        private static string TransactionRequestText(string SenderLogin, string SenderFirstName, string SenderLastName, List<BucketItem> PurchaseList, string Message, ShippingAdress Address)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(ShopApp.Utility.EmailDesigner.EmailDesigner.PreTransactionRequestHTML);
            message.AppendLine($" <tr> <td colspan=\"8\"> <p><span style=\"color: #ffb24a; font-weight:700;\">{SenderLogin}</span> wysłał Ci ofertę kupna</p> </td> </tr> <!-- HEADER TABLE ROW --> <tr style=\"font-weight: 700;\"><td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"><p>NAZWA OFERTY</p></td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"><p>ILOŚĆ</p></td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"><p>CENA ZA SZTUKĘ</p></td> <td colspan=\"2\" style=\" border: 1px solid #9e9e9d;padding: 5px;\"><p>RAZEM</p></td></tr>");

            foreach(var item in PurchaseList)
            {
                if(item.Offer != null)
                {
                    message.AppendLine($"<tr> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>{item.Offer.Title}</p> </td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>{item.Quantity}</p> </td>");
                    message.AppendLine($" <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>{item.Offer.Price} zł</p> </td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p> {item.TotalPrice} zł</p> </td> </tr>");
                    //message.AppendLine($"<img src=\"{ item.Offer.OfferPictures.First().PathToFile }\" style = \"height:120px;width:120px;\" alt = \"Item image\" />");
                    //message.AppendLine($"{item.Offer.Title}");
                    //message.AppendLine($"W ilości: {item.Quantity}");
                }
                else
                {
                    message.AppendLine($"<tr> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>{item.Bundle.Title}</p> </td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>Zestaw</p> </td>");
                    message.AppendLine($"<td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p>{item.Bundle.BundlePrice} zł</p> </td> <td colspan=\"2\" style=\"border: 1px solid #9e9e9d; padding: 5px;\"> <p> {item.TotalPrice} zł</p> </td> </tr>");
                    //message.AppendLine($"<img src=\"{ item.Bundle.Offers.First().OfferPictures.First().PathToFile }\" style = \"height:120px;width:120px;\" alt = \"Item image\" />");
                    //message.AppendLine($"{item.Bundle.Title}");
                }
            }

            message.AppendLine("<tr> <td> <p></p> </td> </tr> <tr style=\"border-top: 1px solid #9e9e9d;\"> <td colspan=\"8\" style=\"width: 100%; font-weight: 700;\"> <p>Dane adresowe kupującego</p> </td> </tr>");
            message.AppendLine($"<tr><td colspan=\"8\" style=\"width: 100%;\"> <p style=\"margin-top: 5px;\">{SenderFirstName} {SenderLastName}</p> </td> </tr> <tr> <td colspan=\"8\" style=\"width: 100%;\"> <p style=\"margin-top: 5px;\">ul. {Address.City} {Address.PremisesNumber ?? '/' + Address.PremisesNumber}</p> </td> </tr>");
            message.AppendLine($"<tr><td colspan=\"8\" style=\"width: 100%;\"> <p style=\"margin-top: 5px;\">{Address.PostalCode} {Address.City}</p> </td> </tr> <tr> <td colspan=\"8\" style=\"width: 100%;\"> <p style=\"margin-top: 5px;\">{Address.Country}</p> </td> </tr>");
            message.AppendLine($"<tr style=\"border-top: 1px solid #9e9e9d;\"> <td colspan=\"8\" style=\"width: 100%; font-weight: 700;\"> <p>Wiadomość od kupującego</p> </td> </tr><tr><td style=\"border-bottom: 1px solid #9e9e9d;\"></td> <td colspan=\"6\" style=\"width: 100%; padding: 5px;\"> <p style=\"margin: 5px\">{Message}</p> </td> <td></td> </tr>");

            message.AppendLine(ShopApp.Utility.EmailDesigner.EmailDesigner.PostTransactionRequestHTML);

            return message.ToString();
        }

        // The end of every message (can be made in gmail options too).
        private static string EndOfEmail()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<pre>");
            message.AppendLine("-----------------------------------------------");
            message.AppendLine("");
            message.AppendLine("Best regards,");
            message.AppendLine("Zakupowo Team<pre>");

            return message.ToString();
        }

        private static string EncryptEmail(string email)
        {
            StringBuilder result = new StringBuilder();

            int atIndex = email.IndexOf('@');

            string beforeAt = email.Substring(0, atIndex);
            string afterAt = email.Substring(atIndex + 1);

            string[] partsBeforeAt = beforeAt.Split('.');
            string[] partsAfterAt = afterAt.Split('.');

            for (int i = 0; i < partsBeforeAt.Length; i++)
            {
                result.Append(Cryptographing.Encrypt(partsBeforeAt[i]));
                if (i < partsBeforeAt.Length - 1)
                    result.Append('.');
            }

            result.Append('@');

            for (int i = 0; i < partsAfterAt.Length; i++)
            {
                result.Append(Cryptographing.Encrypt(partsAfterAt[i]));
                if (i < partsAfterAt.Length - 1)
                    result.Append('.');
            }

            return result.ToString();
        }

        public static string DecryptEmail(string decryptedEmail)
        {
            StringBuilder result = new StringBuilder();

            int atIndex = decryptedEmail.IndexOf('@');

            string beforeAt = decryptedEmail.Substring(0, atIndex);
            string afterAt = decryptedEmail.Substring(atIndex + 1);

            string[] partsBeforeAt = beforeAt.Split('.');
            string[] partsAfterAt = afterAt.Split('.');

            for (int i = 0; i < partsBeforeAt.Length; i++)
            {
                partsBeforeAt[i] = partsBeforeAt[i].Replace(' ', '+');
                result.Append(Cryptographing.Decrypt(partsBeforeAt[i]));
                if (i < partsBeforeAt.Length - 1)
                    result.Append('.');
            }

            result.Append('@');

            for (int i = 0; i < partsAfterAt.Length; i++)
            {
                partsAfterAt[i] = partsAfterAt[i].Replace(' ', '+');
                result.Append(Cryptographing.Decrypt(partsAfterAt[i]));
                if (i < partsAfterAt.Length - 1)
                    result.Append('.');
            }

            return result.ToString();
        }

        public static string PasswordResetText(string PasswordResetCode)
        {
            StringBuilder message = new StringBuilder();


            message.AppendLine(ShopApp.Utility.EmailDesigner.EmailDesigner.PrePasswordResetCodeHTML);

            message.AppendLine($"<tr> <td style=\"border-bottom: 1px solid black;\"> <p style=\"margin-top: 5px;\"><span style=\"font-size: 2.6rem; font-weight: 700;\">{PasswordResetCode}</span></p> </td> </tr>");

            message.AppendLine(ShopApp.Utility.EmailDesigner.EmailDesigner.PostPasswordResetCodeHTML);


            return message.ToString();
        }
    }
}