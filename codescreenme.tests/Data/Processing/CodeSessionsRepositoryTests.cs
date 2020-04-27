using codescreenme.Data;
using codescreenme.Data.Processing;
using NUnit.Framework;
using System.Linq;

namespace codescreenme.tests.Data.Processing
{
  class InMemoryCodeSessionsRepositoryTests
  {
    private ICodeSessionsRepository codeSessionsRepository;
    private const string User = "[unit-test]";

    [SetUp]
    public void Setup()
    {
      this.codeSessionsRepository = new InMemoryCodeSessionsRepository();
    }

    [Test, Order(1)]
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

    [Test, Order(2)]
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
  }
}
