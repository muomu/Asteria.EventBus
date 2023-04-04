using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Asteria.EventBus
{
    internal class Launch : IHostedService
    {

        

        public Launch(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach(var actionDescriptor in actionDescriptorCollectionProvider.ActionDescriptors.Items)
            {
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
