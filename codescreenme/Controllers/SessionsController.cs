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
      string user = this.userRepository.GetCurrentUserId();
      var sessions = this.codeSessionsRepository.GetUserOwnedSessions(user);

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
      string user = this.userRepository.GetCurrentUserId();
      if (string.IsNullOrEmpty(user))
      {
        return new StatusCodeResult(500);
      }

      CodeSession newCodeSession = new CodeSession()
      {
        Code = codeSession == null ? string.Empty : codeSession.Code,
        DateCreated = codeSession == null ? DateTime.UtcNow : codeSession.DateCreated,
        Owner = user,
        UserInControl = user,
        CodeSyntax = "text/x-csharp",
        Participants = new List<string>(new string[] { user })
      };

      this.codeSessionsRepository.CreateNewSession(newCodeSession);

      return new ActionResult<CodeSession>(newCodeSession);
    }

    [HttpDelete("{id}")]
    public ActionResult<bool> Delete(string id)
    {
      string user = this.userRepository.GetCurrentUserId();

      var result = this.codeSessionsRepository.RemoveSession(user, id);
      return new ActionResult<bool>(result);
    }
  }
}