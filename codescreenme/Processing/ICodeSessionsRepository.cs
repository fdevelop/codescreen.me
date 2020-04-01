using codescreenme.Data;
using System.Collections.Generic;

namespace codescreenme.Processing
{
  public interface ICodeSessionsRepository
  {
    void CreateNewSession(CodeSession codeSession);
    CodeConnection GetSessionById(string user, string id);
    bool UpdateSession(string user, string id, string code);
    bool UpdateSession(string user, string id, CodeCursor codeCursor);
    bool RemoveSession(string user, string id);
    IEnumerable<CodeSession> GetUserOwnedSessions(string user);
  }
}