using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using codescreenme.Data;
using codescreenme.Processing;
using Microsoft.AspNetCore.Mvc;

namespace codescreenme.Controllers
{
  [Route("api/users")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private IUserRepository userRepository;
    private ICodeSessionsRepository codeSessionsRepository;

    public UserController(IUserRepository userRepository, ICodeSessionsRepository codeSessionsRepository)
    {
      this.userRepository = userRepository;
      this.codeSessionsRepository = codeSessionsRepository;
    }

    [HttpGet("current")]
    public ActionResult<User> Get()
    {
      return this.userRepository.GetCurrentUser();
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetById(string id)
    {
      return this.userRepository.GetUserById(id);
    }

    [HttpPut("{id}")]
    public ActionResult<bool> PutUser(string id, [FromBody]User user)
    {
      return this.userRepository.UpdateUser(id, user);
    }
  }
}