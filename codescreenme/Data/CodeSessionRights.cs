using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data
{
  public class CodeSessionRights
  {
    public bool CanAdministrate { get; private set; }
    public bool CanEdit { get; private set; }

    public CodeSessionRights(bool canAdministrate, bool canEdit)
    {
      this.CanAdministrate = canAdministrate;
      this.CanEdit = canEdit;
    }
  }
}
