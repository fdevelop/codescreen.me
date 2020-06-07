﻿using codescreenme.Data;
using codescreenme.Data.Processing;
using Microsoft.Extensions.Logging;
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
    private ILoggerFactory consoleLogger;

    internal override void SetCodeRepo()
    {
      this.mockProvider = new Mock<IPersistentStorageProvider>();
      this.consoleLogger = LoggerFactory.Create(builder => builder.AddConsole());
      this.codeSessionsRepository = new PersistentCodeSessionsRepository(mockProvider.Object, consoleLogger);
      
      PersistentCodeSessionsRepository.ResetState();
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

