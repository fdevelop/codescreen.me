using codescreenme.Data;
using codescreenme.Data.Processing;
using NUnit.Framework;
using System;
using System.Linq;

namespace codescreenme.tests.Data.Processing
{
  abstract class CodeSessionsRepositoryTests
  {
    protected ICodeSessionsRepository codeSessionsRepository;
    private const string User = "[unit-test]";

    [SetUp]
    public void Setup()
    {
      this.SetCodeRepo();
    }

    internal abstract void SetCodeRepo();

    [Test]
    public void TestAdding()
    {
      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      var result = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(result.Count(), 1);
      Assert.AreEqual(result.First().Id, id);
    }

    [Test]
    public void TestRemoving()
    {
      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      var removeOp = this.codeSessionsRepository.RemoveSession(User, id);

      var currentState = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(removeOp, true);
      Assert.AreEqual(currentState.Count(), 0);
    }

    [Test]
    public void TestGetting()
    {
      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      var result = this.codeSessionsRepository.GetSessionById(User, id);

      Assert.IsNotNull(result);
      Assert.AreEqual(result.CodeSession.Id, id);

      CodeSession cs2 = new CodeSession()
      {
        Owner = User
      };

      this.codeSessionsRepository.CreateNewSession(cs2);

      var result2 = this.codeSessionsRepository.GetAllSessionsByDateRange(DateTime.MinValue, DateTime.UtcNow);

      Assert.AreEqual(result2.Count(), 2);
    }

    [Test]
    public void TestUpdate()
    {
      const string Old = "Hello World!";
      const string New = "Nope";

      CodeSession cs = new CodeSession()
      {
        Owner = User,
        Code = Old
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      // update code text

      this.codeSessionsRepository.UpdateSession(User, id, New);

      var result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      var onlySession = result.First();

      Assert.AreEqual(onlySession.Code, New);

      // update highlight

      var highlightOne = new CodeCursor() { 
        HighlightFrom = new CodeCursor.CursorPoint() { Ch = 0, Line = 0 },
        HighlightTo = new CodeCursor.CursorPoint() { Ch = 2, Line = 0 }
      };

      this.codeSessionsRepository.UpdateSession(User, id, highlightOne);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First();

      Assert.AreEqual(onlySession.CodeHighlights.Count, 1);
      Assert.AreEqual(onlySession.CodeHighlights.First().HighlightTo.Ch, 2);

      // update highlight erase
      this.codeSessionsRepository.UpdateSessionEraseHighlights(User, id);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First();

      Assert.AreEqual(onlySession.CodeHighlights.Count, 0);

      // update syntax
      const string NewSyntax = "nothing";
      this.codeSessionsRepository.UpdateSessionSyntax(User, id, NewSyntax);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First();

      Assert.AreEqual(onlySession.CodeSyntax, NewSyntax);

      // update user in control
      const string NewUser = "newUser";
      this.codeSessionsRepository.UpdateSessionUserInControl(User, id, NewUser);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First();

      Assert.AreEqual(onlySession.UserInControl, NewUser);
      Assert.AreEqual(onlySession.Owner, User);
    }
  }
}
