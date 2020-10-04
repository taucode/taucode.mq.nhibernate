using System;

namespace TauCode.Mq.NHibernate.Tests.App.Domain.Notes
{
    public class Note
    {
        private Note()
        {
            
        }

        public Note(
            string userId,
            string subject,
            string body)
        {
            this.Id = Guid.NewGuid().ToString();

            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            this.Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
    }
}
