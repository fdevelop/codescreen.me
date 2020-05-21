using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codescreenme.Data.Processing
{
  public class PersistentCodeSessionsRepository : ICodeSessionsRepository, ICodeUpdateQueue
  {
    private static IDictionary<string, CodeSession> codeSessions;
    private static bool flagCacheLoaded;
    private static object initLock = new object();

    private static HashSet<string> codeSessionUpdates;
    private static readonly object codeUpdatesLock = new object();

    private readonly IPersistentStorageProvider persistentStorageProvider;

    static PersistentCodeSessionsRepository()
    {
      codeSessions = new Dictionary<string, CodeSession>();
      codeSessionUpdates = new HashSet<string>();
      flagCacheLoaded = false;
    }

    public PersistentCodeSessionsRepository(IPersistentStorageProvider provider)
    {
      this.persistentStorageProvider = provider;

      this.Init();
    }

    private void Init()
    {
      lock (initLock)
      {
        if (flagCacheLoaded) { return; }

        try
        {
          var listFromDb = this.persistentStorageProvider.GetCodeSessions();

          foreach (var cs in listFromDb)
          {
            if (!codeSessions.ContainsKey(cs.Id))
            {
              codeSessions.Add(cs.Id, cs);
            }
          }

          flagCacheLoaded = true;
        }
        catch (Exception)
        {
          codeSessions.Clear();
          flagCacheLoaded = false;
        }
      }
    }

    public bool ArchiveSession(string user, string id)
    {
      return this.RemoveSession(user, id);
    }

    public void CreateNewSession(CodeSession codeSession)
    {
      codeSessions.Add(codeSession.Id, codeSession);

      try
      {
        this.persistentStorageProvider.AddCodeSession(codeSession);
      }
      catch (Exception ex)
      {
      }
    }

    public IEnumerable<CodeSession> GetAllSessionsByDateRange(DateTime from, DateTime to)
    {
      return codeSessions.Values.Where(cs => cs.DateCreated > from && cs.DateCreated < to);
    }

    private CodeSession GetCodeSessionById(string id)
    {
      if (!codeSessions.ContainsKey(id))
      {
        var csFromDb = this.persistentStorageProvider.GetCodeSession(id);
        if (csFromDb == null)
        {
          return null;
        }

        codeSessions.Add(id, csFromDb);
      }

      return codeSessions[id];
    }

    public CodeConnection GetSessionById(string user, string id)
    {
      var element = this.GetCodeSessionById(id);
      if (element == null)
      {
        return null;
      }

      if (!element.Participants.Contains(user))
      {
        element.Participants.Add(user);
      }

      var codeConnection = new CodeConnection() { CodeSession = element, User = user };
      return codeConnection;
    }

    public IEnumerable<CodeSession> GetUserOwnedSessions(string user)
    {
      return codeSessions.Values.Where(cs => cs.Owner == user);
    }

    public bool RemoveSession(string user, string id)
    {
      if (!codeSessions.ContainsKey(id))
      {
        return false;
      }

      try
      {
        var dbRemove = this.persistentStorageProvider.RemoveCodeSession(id);
        var cacheRemove = codeSessions.Remove(id);

        return dbRemove && cacheRemove;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public bool UpdateSession(string user, string id, string code)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.Code = code;
      this.AddToUpdateQueue(id);

      return true;
    }

    public bool UpdateSession(string user, string id, CodeCursor codeCursor)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeHighlights.Add(codeCursor);
      this.AddToUpdateQueue(id);

      return true;
    }

    public bool UpdateSessionEraseHighlights(string user, string id)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeHighlights.Clear();
      this.AddToUpdateQueue(id);

      return true;
    }

    public bool UpdateSessionSyntax(string user, string id, string syntax)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.CodeSyntax = syntax;
      this.AddToUpdateQueue(id);

      return true;
    }

    public bool UpdateSessionUserInControl(string user, string id, string newUserInControl)
    {
      var e = this.GetCodeSessionById(id);
      if (e == null)
      {
        return false;
      }

      e.UserInControl = newUserInControl;
      this.AddToUpdateQueue(id);

      return true;
    }

    public void AddToUpdateQueue(string id)
    {
      lock (codeUpdatesLock)
      {
        codeSessionUpdates.Add(id);
      }
    }

    public IEnumerable<string> GetUpdateQueueSnapshot()
    {
      lock (codeUpdatesLock)
      {
        var snapshot = codeSessionUpdates.ToArray();
        codeSessionUpdates.Clear();

        return snapshot;
      }
    }

    public void UpdateRecord(string id)
    {
      var csInCache = codeSessions[id];

      this.persistentStorageProvider.UpdateCodeSession(id, csInCache.Code, csInCache.CodeSyntax,
        csInCache.CodeHighlights, csInCache.Participants, csInCache.UserInControl);
    }
  }
}
