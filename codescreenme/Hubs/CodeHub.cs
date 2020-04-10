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
      await Clients.All.SendAsync("ReceiveCodeUpdate", user, sessionId, code);

      this.codeSessionsRepository.UpdateSession(user, sessionId, code);
    }

    public async Task RemoveCodeHighlights(string user, string sessionId)
    {
      await Clients.All.SendAsync("RemoveCodeHighlights", user, sessionId);

      this.codeSessionsRepository.UpdateSessionEraseHighlights(user, sessionId);
    }

    public async Task SetCodeHighlight(string user, string sessionId, CodeCursor codeCursor)
    {
      await Clients.All.SendAsync("SetCodeHighlight", user, sessionId, codeCursor);

      this.codeSessionsRepository.UpdateSession(user, sessionId, codeCursor);
    }

    public async Task ParticipantJoined(string user, string sessionId)
    {
      await Clients.All.SendAsync("ParticipantJoined", user, sessionId);
    }
  }
}
