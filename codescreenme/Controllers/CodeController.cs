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
      User user = this.userRepository.GetCurrentUser();
      var session = this.codeSessionsRepository.GetSessionById(user.Id, id);

      return new JsonResult(session);
    }

    [HttpDelete("{id}/cursor")]
    public ActionResult DeleteCodeCursor(string id)
    {
      User user = this.userRepository.GetCurrentUser();
      return this.codeSessionsRepository.UpdateSessionEraseHighlights(user.Id, id) ? Ok() : StatusCode(500);
    }

    [HttpPut("{id}/cursor")]
    public ActionResult PutCodeCursor(string id, [FromBody]CodeCursor codeCursor)
    {
      User user = this.userRepository.GetCurrentUser();
      return this.codeSessionsRepository.UpdateSession(user.Id, id, codeCursor) ? Ok() : StatusCode(500);
    }

    [HttpPut("{id}/codetext")]
    public ActionResult PutCodeText(string id, [FromBody]string codeText)
    {
      User user = this.userRepository.GetCurrentUser();
      return this.codeSessionsRepository.UpdateSession(user.Id, id, codeText) ? Ok() : StatusCode(500);
    }
  }
}