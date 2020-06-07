using codescreenme.Data;
using codescreenme.Data.Processing;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace codescreenme.tests.Data.Processing
{
  abstract class CodeSessionsRepositoryTests
  {
    protected ICodeSessionsRepository codeSessionsRepository;
    protected const string User = "[unit-test]";

    [SetUp]
    public void Setup()
    {
      this.SetCodeRepo();
    }

    internal abstract void SetCodeRepo();

    [Test]
    public virtual void TestAdding()
    {
      var startCount = this.codeSessionsRepository.GetUserOwnedSessions(User).Count();

      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      var result = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(startCount + 1, result.Count());
      Assert.AreEqual(id, result.First().Id);
    }

    [Test]
    public virtual void TestRemoving()
    {
      var startStateCount = this.codeSessionsRepository.GetUserOwnedSessions(User).Count();

      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      var removeOp = this.codeSessionsRepository.RemoveSession(User, id);

      var currentState = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(true, removeOp);
      Assert.AreEqual(startStateCount, currentState.Count());
    }

    [Test]
    public virtual void TestGetting()
    {
      var startDate = DateTime.UtcNow;
      Thread.Sleep(2000);

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

      var result2 = this.codeSessionsRepository.GetAllSessionsByDateRange(startDate, DateTime.UtcNow);

      Assert.AreEqual(result2.Count(), 2);
    }

    [Test]
    public virtual void TestUpdate()
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
      var onlySession = result.First(s => s.Id == id);

      Assert.AreEqual(New, onlySession.Code);

      // update highlight

      var highlightOne = new CodeCursor() { 
        HighlightFrom = new CodeCursor.CursorPoint() { Ch = 0, Line = 0 },
        HighlightTo = new CodeCursor.CursorPoint() { Ch = 2, Line = 0 }
      };

      this.codeSessionsRepository.UpdateSession(User, id, highlightOne);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First(s => s.Id == id);

      Assert.AreEqual(1, onlySession.CodeHighlights.Count);
      Assert.AreEqual(2, onlySession.CodeHighlights.First().HighlightTo.Ch);

      // update highlight erase
      this.codeSessionsRepository.UpdateSessionEraseHighlights(User, id);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First(s => s.Id == id);

      Assert.AreEqual(0, onlySession.CodeHighlights.Count);

      // update syntax
      const string NewSyntax = "nothing";
      this.codeSessionsRepository.UpdateSessionSyntax(User, id, NewSyntax);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First(s => s.Id == id);

      Assert.AreEqual(NewSyntax, onlySession.CodeSyntax);

      // update user in control
      const string NewUser = "newUser";
      this.codeSessionsRepository.UpdateSessionUserInControl(User, id, NewUser);

      result = this.codeSessionsRepository.GetUserOwnedSessions(User);
      onlySession = result.First(s => s.Id == id);

      Assert.AreEqual(NewUser, onlySession.UserInControl);
      Assert.AreEqual(User, onlySession.Owner);
    }
  }
}
