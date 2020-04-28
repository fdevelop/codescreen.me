using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data.Processing
{
  public class PersistentSqlCodeSessionsRepository : ICodeSessionsRepository
  {
    private IDictionary<string, CodeSession> codeSessions;
    public PersistentSqlCodeSessionsRepository()
    {
      this.codeSessions = new Dictionary<string, CodeSession>();
    }
    public bool ArchiveSession(string user, string id)
    {
      return this.RemoveSession(user, id);
    }

    public void CreateNewSession(CodeSession codeSession)
    {
      this.codeSessions.Add(codeSession.Id, codeSession);
    }

    public IEnumerable<CodeSession> GetAllSessionsByDateRange(DateTime from, DateTime to)
    {
      return this.codeSessions.Values.Where(cs => cs.DateCreated > from && cs.DateCreated < to);
    }

    private CodeSession GetCodeSessionById(string id)
    {
      var element = this.codeSessions[id];
      return element;
    }

    public CodeConnection GetSessionById(string user, string id)
    {
      var element = this.GetCodeSessionById(id);
      if (element == null)
      {
        return null;
      }

      if (!element.Participants.Contains(user))
      {
        element.Participants.Add(user);
      }

      var codeConnection = new CodeConnection() { CodeSession = element, User = user };
      return codeConnection;
    }

    public IEnumerable<CodeSession> GetUserOwnedSessions(string user)
    {
      return this.codeSessions.Values.Where(cs => cs.Owner == user);
    }

    public bool RemoveSession(string user, string id)
    {
      if (!this.codeSessions.ContainsKey(id))
      {
        return false;
      }

      return this.codeSessions.Remove(id);
    }

    public bool UpdateSession(string user, string id, string code)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }
      
      e.Code = code;
      return true;
    }

    public bool UpdateSession(string user, string id, CodeCursor codeCursor)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeHighlights.Add(codeCursor);
      return true;
    }

    public bool UpdateSessionEraseHighlights(string user, string id)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeHighlights.Clear();
      return true;
    }

    public bool UpdateSessionSyntax(string user, string id, string syntax)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeSyntax = syntax;
      return true;
    }

    public bool UpdateSessionUserInControl(string user, string id, string newUserInControl)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.UserInControl = newUserInControl;
      return true;
    }
  }
}
