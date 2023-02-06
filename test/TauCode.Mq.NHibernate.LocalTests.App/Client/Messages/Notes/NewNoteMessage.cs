namespace TauCode.Mq.NHibernate.LocalTests.App.Client.Messages.Notes;

public class NewNoteMessage : IMessage
{
    public string Topic { get; set; }
    public string CorrelationId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string UserId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public ImportanceDto Importance { get; set; }
}