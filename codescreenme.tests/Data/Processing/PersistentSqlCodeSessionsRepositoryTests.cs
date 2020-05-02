using codescreenme.Data;
using codescreenme.Data.Processing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace codescreenme.tests.Data.Processing
{
  class PersistentSqlCodeSessionsRepositoryTests : CodeSessionsRepositoryTests
  {
    private Mock<IPersistentStorageProvider> mockProvider;

    internal override void SetCodeRepo()
    {
      if (this.codeSessionsRepository == null)
      {
        this.mockProvider = new Mock<IPersistentStorageProvider>();
        this.codeSessionsRepository = new PersistentCodeSessionsRepository(mockProvider.Object);
      }
    }

    [Test]
    public override void TestRemoving()
    {
      var startStateCount = this.codeSessionsRepository.GetUserOwnedSessions(User).Count();

      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      this.mockProvider.Setup(m => m.RemoveCodeSession(id)).Returns(true);

      var removeOp = this.codeSessionsRepository.RemoveSession(User, id);

      var currentState = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(true, removeOp);
      Assert.AreEqual(startStateCount, currentState.Count());
    }
  }
}

