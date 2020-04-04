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

    [HttpDelete("{id}/cursor")]
    public ActionResult DeleteCodeCursor(string id)
    {
      string user = this.userRepository.GetCurrentUserId();
      return this.codeSessionsRepository.UpdateSessionEraseHighlights(user, id) ? Ok() : StatusCode(500);
    }

    [HttpPut("{id}/cursor")]
    public ActionResult PutCodeCursor(string id, [FromBody]CodeCursor codeCursor)
    {
      string user = this.userRepository.GetCurrentUserId();
      return this.codeSessionsRepository.UpdateSession(user, id, codeCursor) ? Ok() : StatusCode(500);
    }

    [HttpPut("{id}/codetext")]
    public ActionResult PutCodeText(string id, [FromBody]string codeText)
    {
      string user = this.userRepository.GetCurrentUserId();
      return this.codeSessionsRepository.UpdateSession(user, id, codeText) ? Ok() : StatusCode(500);
    }
  }
}