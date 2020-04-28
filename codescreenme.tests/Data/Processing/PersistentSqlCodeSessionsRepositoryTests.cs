using codescreenme.Data.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace codescreenme.tests.Data.Processing
{
  class PersistentSqlCodeSessionsRepositoryTests : CodeSessionsRepositoryTests
  {
    internal override void SetCodeRepo()
    {
      this.codeSessionsRepository = new PersistentSqlCodeSessionsRepository();
    }
  }
}

