using Infra.Full.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiRaspbian.Tasks
{
  public class Reading : ITaskObject
  {
    public TimeSpan? WaitTimeout => null;

    public Task StartAsync(CancellationToken token)
    {
      while(!token.IsCancellationRequested)
      {
        
      }

      return Task.CompletedTask;
    }
  }
}
