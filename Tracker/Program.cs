using Automatonymous;
using MassTransit;
using MassTransit.MongoDbIntegration.Saga;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
	class Program
	{
		static void Main(string[] args)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.ColoredConsole()
				.CreateLogger();

			var repository = new MongoDbSagaRepository<SagaInstance>("mongodb://test-host", "TST");
			var stateMachine = new TestStateMachine();

			var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.UseSerilog(logger);

				var host = cfg.Host(new Uri("rabbitmq://test-host"), h =>
				{
					h.Username("test");
					h.Password("test");
				});

				cfg.ReceiveEndpoint(host, "test_flow", e =>
				{
					e.PrefetchCount = 8;
					e.UseRetry(Retry.Interval(5, 1000));
					e.StateMachineSaga(stateMachine, repository);
				});


				cfg.ReceiveEndpoint(host, "test_services", e =>
				{
					e.Consumer<FastOperationConsumer>();
				});
			});

			bus.Start();

			Console.WriteLine("Press any key to stop tracker");
			Console.ReadKey();

			bus.Stop();
		}
	}
}
