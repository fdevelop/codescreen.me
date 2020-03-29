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
  [Route("api/code")]
  [ApiController]
  public class CodeController : ControllerBase
  {
    private IUserRepository userRepository;
    private ICodeSessionsRepository codeSessionsRepository;

    public CodeController(IUserRepository userRepository, ICodeSessionsRepository codeSessionsRepository)
    {
      this.userRepository = userRepository;
      this.codeSessionsRepository = codeSessionsRepository;
    }

    [HttpGet("{id}")]
    public ActionResult Get(string id)
    {
      string user = this.userRepository.GetCurrentUserId();
      var session = this.codeSessionsRepository.GetSessionById(user, id);

      return new JsonResult(session);
    }

    [HttpPut("{id}")]
    public ActionResult PutCode(string id, [FromBody]CodeSessionState code)
    {
      string user = this.userRepository.GetCurrentUserId();
      return this.codeSessionsRepository.UpdateSession(user, id, code.Text) ? Ok() : StatusCode(500);
    }
  }
}