using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using codescreenme.Data;
using codescreenme.Processing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace codescreenme.Controllers
{
  [Route("api/sessions")]
  [ApiController]
  public class SessionsController : ControllerBase
  {
    private IUserRepository userRepository;
    private ICodeSessionsRepository codeSessionsRepository;

    public SessionsController(IUserRepository userRepository, ICodeSessionsRepository codeSessionsRepository)
    {
      this.userRepository = userRepository;
      this.codeSessionsRepository = codeSessionsRepository;
    }

    [HttpGet]
    public IEnumerable<CodeSession> Get()
    {
      User user = this.userRepository.GetCurrentUser();
      if (user == null)
      {
        return new CodeSession[0];
      }

      var sessions = this.codeSessionsRepository.GetUserOwnedSessions(user.Id);

      return sessions;
    }

    [HttpGet("{id}")]
    public string Get(string id)
    {
      return string.Empty;
    }

    [HttpPost()]
    public ActionResult<CodeSession> Post(CodeSession codeSession)
    {
      User user = this.userRepository.GetCurrentUser();
      if (user == null)
      {
        return new StatusCodeResult(500);
      }

      CodeSession newCodeSession = new CodeSession()
      {
        Code = codeSession == null ? string.Empty : codeSession.Code,
        DateCreated = codeSession == null ? DateTime.UtcNow : codeSession.DateCreated,
        Owner = user.Id,
        Participants = new List<string>(new string[] { user.Id })
      };

      this.codeSessionsRepository.CreateNewSession(newCodeSession);

      return new ActionResult<CodeSession>(newCodeSession);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
      User user = this.userRepository.GetCurrentUser();

      var result = this.codeSessionsRepository.RemoveSession(user.Id, id);
      return new ActionResult<bool>(result);
    }
  }
}