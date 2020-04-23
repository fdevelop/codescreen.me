using codescreenme.Data;
using System;
using System.Collections.Generic;

namespace codescreenme.Processing
{
  public interface ICodeSessionsRepository
  {
    void CreateNewSession(CodeSession codeSession);
    CodeConnection GetSessionById(string user, string id);
    bool UpdateSession(string user, string id, string code);
    bool UpdateSession(string user, string id, CodeCursor codeCursor);
    bool UpdateSessionSyntax(string user, string id, string syntax);
    bool UpdateSessionEraseHighlights(string user, string id);
    bool UpdateSessionUserInControl(string user, string id, string newUserInControl);
    bool RemoveSession(string user, string id);
    bool ArchiveSession(string user, string id);
    IEnumerable<CodeSession> GetUserOwnedSessions(string user);
    IEnumerable<CodeSession> GetAllSessionsByDateRange(DateTime from, DateTime to);
  }
}