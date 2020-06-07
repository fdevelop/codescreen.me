using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data.Processing
{
  public interface ICodeUpdateQueue
  {
    void AddToUpdateQueue(string id);
    IEnumerable<string> GetUpdateQueueSnapshot();
    void UpdateRecord(string id);
  }
}
