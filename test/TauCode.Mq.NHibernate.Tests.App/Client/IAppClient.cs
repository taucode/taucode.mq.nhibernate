using System.Collections.Generic;
using System.Threading.Tasks;
using TauCode.Mq.NHibernate.Tests.App.Client.Dto.Notes;

namespace TauCode.Mq.NHibernate.Tests.App.Client
{
    public interface IAppClient
    {
        Task<IList<NoteDto>> GetUserNotes(string userId);
    }
}
