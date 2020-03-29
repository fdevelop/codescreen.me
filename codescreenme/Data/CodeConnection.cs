using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data
{
  public class CodeConnection
  {
    public CodeSession CodeSession { get; set; }
    public string User { get; set; }
    public CodeSessionRole Role { get; set; }
  }
}
