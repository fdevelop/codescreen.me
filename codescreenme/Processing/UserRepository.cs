using codescreenme.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace codescreenme.Processing
{
  public class UserRepository : IUserRepository
  {
    private IDictionary<string, User> activeUsers;

    private readonly IHttpContextAccessor httpContextAccessor;

    public UserRepository(IHttpContextAccessor httpContextAccessor)
    {
      this.httpContextAccessor = httpContextAccessor;
      this.activeUsers = new Dictionary<string, User>();
    }

    public User GetCurrentUser()
    {
      string user;
      if (this.httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("UniqueUser", out user))
      {
        return this.GetUserById(user);
      }

      var newUser = this.InitializeNewUser();

      return newUser;
    }

    private User InitializeNewUser()
    {
      var user = Guid.NewGuid().ToString();

      CookieOptions option = new CookieOptions();
      option.Expires = DateTime.MaxValue;
      this.httpContextAccessor.HttpContext.Response.Cookies.Append("UniqueUser", user, option);

      CookieOptions option2 = new CookieOptions();
      option2.Expires = DateTime.MaxValue;
      this.httpContextAccessor.HttpContext.Response.Cookies.Append("UniqueUserDisplayName", user, option2);

      return new User() { Id = user, DisplayName = user };
    }

    public User GetUserById(string id)
    {
      if (!this.activeUsers.ContainsKey(id))
      {
        string userDisplayName;
        if (!this.httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("UniqueUserDisplayName", out userDisplayName))
        {
          userDisplayName = id;
        }

        this.activeUsers[id] = new User() { Id = id, DisplayName = userDisplayName };
      }
      
      return this.activeUsers[id];
    }

    public bool UpdateUser(string id, User user)
    {
      var existingUser = this.GetUserById(id);
      var newUser = (User)user.Clone();

      if (existingUser == null)
      {
        return false;
      }

      if (existingUser.DisplayName != newUser.DisplayName)
      {
        string finalValue = HttpUtility.JavaScriptStringEncode(newUser.DisplayName);

        CookieOptions option2 = new CookieOptions();
        option2.Expires = DateTime.MaxValue;
        this.httpContextAccessor.HttpContext.Response.Cookies.Append("UniqueUserDisplayName", finalValue, option2);
      }

      return true;
    }
  }
}
