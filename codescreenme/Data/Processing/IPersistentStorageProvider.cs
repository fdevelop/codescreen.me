using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data.Processing
{
  public interface IPersistentStorageProvider
  {
    bool AddCodeSession(CodeSession codeSession);
    bool UpdateCodeSession(string id, string code, string syntax, IList<CodeCursor> highlights, IList<string> participants, string userInControl);
    bool RemoveCodeSession(string id);
    CodeSession GetCodeSession(string id);
    IEnumerable<CodeSession> GetCodeSessions();
  }
}
