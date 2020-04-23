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
      if (element == null)
      {
        return null;
      }

      var codeConnection = new CodeConnection() { CodeSession = element, User = user };

      if (!codeConnection.CodeSession.Participants.Contains(user))
      {
        codeConnection.CodeSession.Participants.Add(user);
      }
      
      return codeConnection;
    }

    public bool UpdateSession(string user, string id, CodeCursor codeCursor)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.CodeHighlights.Add(codeCursor);

      return true;
    }

    public bool UpdateSessionSyntax(string user, string id, string syntax)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.CodeSyntax = syntax;

      return true;
    }

    public bool UpdateSessionEraseHighlights(string user, string id)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.CodeHighlights.Clear();

      return true;
    }

    public bool UpdateSession(string user, string id, string code)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.Code = code;

      return true;
    }

    public bool UpdateSessionUserInControl(string user, string id, string newUserInControl)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      if (element == null)
        return false;

      element.UserInControl = newUserInControl;

      return true;
    }

    public bool RemoveSession(string user, string id)
    {
      var element = this.codeSessions.FirstOrDefault(cs => cs.Id == id);
      return element == null ? false : this.codeSessions.Remove(element);
    }

    public bool ArchiveSession(string user, string id)
    {
      return this.RemoveSession(user, id);
    }

    public IEnumerable<CodeSession> GetUserOwnedSessions(string user)
    {
      return this.codeSessions.Where(cs => cs.Owner == user);
    }

    public IEnumerable<CodeSession> GetAllSessionsByDateRange(DateTime from, DateTime to)
    {
      return this.codeSessions.Where(cs => cs.DateCreated > from && cs.DateCreated < to);
    }
  }
}
