using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data
{
  public class User : ICloneable
  {
    public string Id { get; set; }
    public string DisplayName { get; set; }

    public object Clone()
    {
      return new User()
      {
        Id = this.Id,
        DisplayName = this.DisplayName
      };
    }
  }
}
