using codescreenme.Data;
using codescreenme.Data.Processing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace codescreenme.tests.Data.Processing
{
  class PersistentSqlCodeSessionsRepositorySqlOfflineTests : CodeSessionsRepositoryTests
  {
    private Mock<IPersistentStorageProvider> mockProvider;
    private ILoggerFactory consoleLogger;

    internal override void SetCodeRepo()
    {
      this.mockProvider = new Mock<IPersistentStorageProvider>();
      this.consoleLogger = LoggerFactory.Create(builder => builder.AddConsole());
      this.codeSessionsRepository = new PersistentCodeSessionsRepository(mockProvider.Object, consoleLogger);

      PersistentCodeSessionsRepository.ResetState();
    }

    [Test]
    public override void TestAdding()
    {
      var startCount = this.codeSessionsRepository.GetUserOwnedSessions(User).Count();

      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.mockProvider.Setup(m => m.AddCodeSession(cs)).Throws(new InvalidOperationException());

      this.codeSessionsRepository.CreateNewSession(cs);

      var result = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(startCount + 1, result.Count());
      Assert.AreEqual(id, result.First().Id);
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
            
      this.mockProvider.Setup(m => m.RemoveCodeSession(id)).Throws(new InvalidOperationException());

      var removeOp = this.codeSessionsRepository.RemoveSession(User, id);

      var currentState = this.codeSessionsRepository.GetUserOwnedSessions(User);

      Assert.AreEqual(false, removeOp);
      Assert.AreEqual(startStateCount, currentState.Count());
    }

    [Test]
    public override void TestGetting()
    {
      var startDate = DateTime.UtcNow;
      Thread.Sleep(2000);

      CodeSession cs = new CodeSession()
      {
        Owner = User
      };
      var id = cs.Id;

      this.codeSessionsRepository.CreateNewSession(cs);

      this.mockProvider.Setup(m => m.GetCodeSession(id)).Throws(new InvalidOperationException());

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
  }
}
