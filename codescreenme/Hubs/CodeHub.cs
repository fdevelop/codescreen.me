using codescreenme.Data;
using codescreenme.Processing;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Hubs
{
  public class CodeHub : Hub
  {
    private IUserRepository userRepository;
    private ICodeSessionsRepository codeSessionsRepository;

    public CodeHub(IUserRepository userRepository, ICodeSessionsRepository codeSessionsRepository)
    {
      this.userRepository = userRepository;
      this.codeSessionsRepository = codeSessionsRepository;
    }

    public async Task ReceiveCodeUpdate(string user, string sessionId, string code)
    {
      this.codeSessionsRepository.UpdateSession(user, sessionId, code);
      await Clients.All.SendAsync("ReceiveCodeUpdate", user, sessionId, code);
    }

    public async Task ReceiveCodeSyntaxUpdate(string user, string sessionId, string syntax)
    {
      this.codeSessionsRepository.UpdateSessionSyntax(user, sessionId, syntax);
      await Clients.All.SendAsync("ReceiveCodeSyntaxUpdate", user, sessionId, syntax);
    }

    public async Task RemoveCodeHighlights(string user, string sessionId)
    {
      this.codeSessionsRepository.UpdateSessionEraseHighlights(user, sessionId);
      await Clients.All.SendAsync("RemoveCodeHighlights", user, sessionId);
    }

    public async Task SetCodeHighlight(string user, string sessionId, CodeCursor codeCursor)
    {
      this.codeSessionsRepository.UpdateSession(user, sessionId, codeCursor);
      await Clients.All.SendAsync("SetCodeHighlight", user, sessionId, codeCursor);
    }

    public async Task ParticipantJoined(string user, string sessionId)
    {
      await Clients.All.SendAsync("ParticipantJoined", user, sessionId);
    }

    public async Task SetUserInControl(string user, string sessionId, string newUser)
    {
      this.codeSessionsRepository.UpdateSessionUserInControl(user, sessionId, newUser);
      await Clients.All.SendAsync("SetUserInControl", user, sessionId, newUser);
    }
  }
}
