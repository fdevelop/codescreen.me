using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data
{
  public class CodeCursor
  {
    public class CursorPoint
    {
      public int Line { get; set; }
      public int Ch { get; set; }
    }

    public CursorPoint SelectionFrom { get; set; }
    public CursorPoint SelectionTo { get; set; }

    public CodeCursor()
    {
      this.SelectionFrom = new CursorPoint();
      this.SelectionTo = new CursorPoint();
    }
  }
}
