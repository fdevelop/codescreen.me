using codescreenme.Data.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace codescreenme.HostedServices
{
  public class PersistentDataUpdaterHostedService : IHostedService, IDisposable
  {
    private readonly static TimeSpan DefaultFrequency = TimeSpan.FromMinutes(1);

    private Timer timer;

    private readonly IServiceScopeFactory scopeFactory;

    public PersistentDataUpdaterHostedService(IServiceScopeFactory scopeFactory)
    {
      this.scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      this.timer = new Timer(DoWork, null, TimeSpan.Zero, DefaultFrequency);
      
      return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
      using (var scope = scopeFactory.CreateScope())
      {
        var codeSessionsRepo = scope.ServiceProvider.GetRequiredService<ICodeSessionsRepository>();

        var updateQueueIds = (codeSessionsRepo as ICodeUpdateQueue).GetUpdateQueueSnapshot();

        foreach (var id in updateQueueIds)
        {
          try
          {
            (codeSessionsRepo as ICodeUpdateQueue).UpdateRecord(id);
          }
          catch (Exception)
          {
            (codeSessionsRepo as ICodeUpdateQueue).AddToUpdateQueue(id);
          }
        }
      }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      this.timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;
    }
    public void Dispose()
    {
      this.timer?.Dispose();
    }
  }
}
