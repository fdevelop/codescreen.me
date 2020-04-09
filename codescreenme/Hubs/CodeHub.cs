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
  }
}
