using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Svyaznoy.Core.Entites;
using Svyaznoy.Core.Web;

namespace Svyaznoy.Core.Mail
{
    public struct ServerCredentials
    {
        public string Host { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Password { get; set; }

        public bool SSL { get; set; }
    }

    public class Message
    {
        private List<Attachment> _attachments;

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public List<Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<Attachment>();
                }
                return _attachments;
            }
            set { _attachments = value; }
        }
    }

    public class Attachment: DataFile
    {
        
    }

    public static class EmailAgent
    {
        public static void Send(ServerCredentials server, Message message, MailAddress from, IEnumerable<MailAddress> to, IEnumerable<MailAddress> cc = null)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (@from == null) throw new ArgumentNullException("from");
            if (to.IsNullOrEmpty()) throw new ArgumentNullException("to");

            var streams = new List<IDisposable>();

            try
            {
                var client = new SmtpClient(server.Host, server.Port)
                    {
                        Credentials = string.IsNullOrWhiteSpace(server.User)
                                          ? new NetworkCredential()
                                          : new NetworkCredential(server.User, server.Password),
                        EnableSsl = server.SSL
                    };

                var mail = new MailMessage()
                    {
                        From = from,
                        Subject = message.Subject,
                        Body = message.Body,
                        IsBodyHtml = message.IsHtml
                    };

                to.ForEachNotNull(a => mail.To.Add(a));

                if (cc.HasValues())
                {
                    cc.ForEachNotNull(a => mail.CC.Add(a));
                }

                if (message.Attachments.HasValues())
                {
                    message.Attachments.ForEachNotNull(
                        a =>
                            {
                                var ms = new MemoryStream(a.Body);
                                streams.Add(ms);
                                mail.Attachments.Add(new System.Net.Mail.Attachment(ms, a.Name, a.Mime));
                            });
                }

                client.Send(mail);
            }
            finally
            {
                streams.ForEachNotNull(s => s.Dispose());
            }
        }

        public static void SplitIntoSubjectAndBody(string message, out string subject, out string body)
        {
            int index = message.IndexOfAny(new char[] { '\r', '\n' });
            subject = index == -1 ? null : message.Substring(0, index);

            body = index == -1 ? message : message.Substring(index, message.Length - index);
        }
    }
}