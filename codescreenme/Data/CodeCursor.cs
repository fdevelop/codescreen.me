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

    public CursorPoint HighlightFrom { get; set; }
    public CursorPoint HighlightTo { get; set; }

    public CodeCursor()
    {
      this.HighlightFrom = new CursorPoint();
      this.HighlightTo = new CursorPoint();
    }
  }
}
