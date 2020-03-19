using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BugTracker.Utilities
{
    public class Email
    {
        private const string PassPath = @"C:\Users\kevsm\source\repos\BugTracker\BugTracker\Utilities\confidential.txt";
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string Username = "ksmeeks0001@gmail.com";
        private const string SendingAddress = "bugtrackingsoftware@gmail.com";
        private const string RegistrationEmailPath = @"C:\Users\kevsm\source\repos\BugTracker\BugTracker\Utilities\RegistrationEmail.txt";
        public static void SendPendingEmail(PendingRegistration pending)
        {
            string password;
            string RegistrationEmail;
            using (StreamReader sr = new StreamReader(PassPath))
            {
                password = sr.ReadLine();
            }
            using (StreamReader sr = new StreamReader(RegistrationEmailPath))
            {
                RegistrationEmail = sr.ReadToEnd();
            }
                SmtpClient client = new SmtpClient(SmtpHost, SmtpPort);
                client.Credentials = new NetworkCredential(Username, password);
                client.EnableSsl = true;
            MailMessage message = new MailMessage()
            {
                From = new MailAddress(SendingAddress),
                Subject = "Bug Tracker Registration",
                Body = RegistrationEmail
                };
                message.To.Add(pending.Email);

                client.Send(message);
            
        }
 
    }
}