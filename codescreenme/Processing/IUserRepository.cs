using codescreenme.Data;

namespace codescreenme.Processing
{
  public interface IUserRepository
  {
    User GetCurrentUser();
    User GetUserById(string id);
    bool UpdateUser(string id, User user);
  }
}