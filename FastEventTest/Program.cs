using Contracts;
using MassTransit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastEventTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.ColoredConsole()
				.CreateLogger();

			var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.UseSerilog(logger);
				var host = cfg.Host(new Uri("rabbitmq://test-host"), h =>
				{
					h.Username("test");
					h.Password("test");
				});
			});

			bus.Start();

			ConsoleKey k;
			while ((k = Console.ReadKey().Key) != ConsoleKey.Q)
			{
				switch (k)
				{
					case ConsoleKey.A:
						{
							var id = Guid.NewGuid();
							Console.WriteLine();
							Console.WriteLine($"Req ID: {id}");
							bus.Publish(new Create { RequestId =  id});
						}
						break;
					case ConsoleKey.B:
						{
							Console.Write("Enter ID to complete: ");
							var line = Console.ReadLine();
							var id = Guid.Parse(line);
							bus.Publish(new FastOperationCompleted { CorrelationId = id });
						}
						break;
				}
			}

			bus.Stop();
		}
	}
}
