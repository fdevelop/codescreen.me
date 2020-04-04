using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codescreenme.Data
{
  [Serializable]
  public class CodeSession
  {
    public string Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Code { get; set; }
    public IList<CodeCursor> CodeHighlights { get; set; }
    public string Owner { get; set; }
    public IList<string> Participants { get; set; }

    public CodeSession()
    {
      this.Id = this.GenerateId();
      this.DateCreated = DateTime.UtcNow;
      this.Participants = new List<string>();
      this.CodeHighlights = new List<CodeCursor>();
    }

    private const int IdLength = 6;

    private string GenerateId()
    {
      Random r = new Random((int)(DateTime.Now.Ticks % (int.MaxValue - 1)));
      var sb = new StringBuilder();
      for (int i = 0; i < IdLength; i++)
      {
        int option = r.Next(3);
        int n = 0;
        if (option == 0)
        {
          n = r.Next('a', 'z');

        }
        else if (option == 1)
        {
          n = r.Next('A', 'Z');
        }
        else
        {
          n = r.Next('0', '9');
        }
        char c = (char)n;
        sb.Append(c);
      }

      return sb.ToString();
    }
  }
}
