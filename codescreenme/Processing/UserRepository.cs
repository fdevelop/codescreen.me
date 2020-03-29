using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Processing
{
  public class UserRepository : IUserRepository
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
      string user;
      if (this._httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("UniqueUser", out user))
      {
        return user;
      }

      user = Guid.NewGuid().ToString();

      CookieOptions option = new CookieOptions();
      option.Expires = DateTime.MaxValue;
      this._httpContextAccessor.HttpContext.Response.Cookies.Append("UniqueUser", user, option);

      return user;
    }
  }
}
