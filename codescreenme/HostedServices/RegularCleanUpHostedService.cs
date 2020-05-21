using codescreenme.Data.Processing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace codescreenme.HostedServices
{
  public class RegularCleanUpHostedService : IHostedService, IDisposable
  {
    public readonly static TimeSpan DefaultSessionLifetime = TimeSpan.FromHours(24);

    private readonly static TimeSpan DefaultFrequency = TimeSpan.FromHours(1);

    private Timer timer;
    private int executionCount = 0;

    private ICodeSessionsRepository codeSessionsRepository;

    public RegularCleanUpHostedService(ICodeSessionsRepository codeSessionsRepository)
    {
      this.codeSessionsRepository = codeSessionsRepository;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      this.timer = new Timer(DoWork, null, TimeSpan.Zero, DefaultFrequency);

      return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
      var count = Interlocked.Increment(ref executionCount);

      var allSessionsToCleanUp = this.codeSessionsRepository.GetAllSessionsByDateRange(DateTime.MinValue, DateTime.UtcNow.Subtract(DefaultSessionLifetime));
      foreach (var sessionId in allSessionsToCleanUp.Select(cs => cs.Id).ToArray())
      {
        this.codeSessionsRepository.ArchiveSession("[system]", sessionId);
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
