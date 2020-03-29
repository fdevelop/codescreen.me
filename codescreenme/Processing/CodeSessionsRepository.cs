using codescreenme.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Processing
{
  public class CodeSessionsRepository : ICodeSessionsRepository
  {
    private IList<CodeSession> codeSessions { get; set; }

    public CodeSessionsRepository()
    {
      this.codeSessions = new List<CodeSession>();
    }

    public CodeSessionsRepository(IEnumerable<CodeSession> codeSessions)
    {
      this.codeSessions = new List<CodeSession>(codeSessions);
    }

    public void CreateNewSession(CodeSession codeSession)
    {
      this.codeSessions.Add(codeSession);
    }

    public CodeConnection GetSessionById(string user, string id)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      var codeConnection = new CodeConnection() { CodeSession = element, Role = element.Owner == user ? CodeSessionRole.Admin : CodeSessionRole.Guest, User = user };

      if (codeConnection.Role == CodeSessionRole.Guest)
      {
        if (!codeConnection.CodeSession.Participants.Contains(user))
        {
          codeConnection.CodeSession.Participants.Add(user);
        }
      }

      return codeConnection;
    }

    public bool UpdateSession(string user, string id, string code)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.Code = code;

      return true;
    }

    public bool RemoveSession(string user, string id)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      return element == null ? false : this.codeSessions.Remove(element);
    }

    public IEnumerable<CodeSession> GetUserOwnedSessions(string user)
    {
      return this.codeSessions.Where(cs => cs.Owner == user);
    }
  }
}
