using Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracker
{
	public class FastOperationConsumer : IConsumer<StartFastOperation>
	{
		public Task Consume(ConsumeContext<StartFastOperation> context)
		{
			Console.WriteLine("Fast operation invoked");
			context.Publish(new FastOperationCompleted { CorrelationId = context.Message.CorrelationId });
			return Task.FromResult(0);
		}
	}
}
