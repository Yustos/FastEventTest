using Automatonymous;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
	public class TestStateMachine : MassTransitStateMachine<SagaInstance>
	{
		public TestStateMachine()
		{
			InstanceState(x => x.CurrentState);

			Event(() => Created, x => x.CorrelateById(c => c.Message.RequestId).SelectId(ctx => ctx.Message.RequestId));
			Event(() => Completed, x => x.CorrelateById(c => c.Message.CorrelationId).OnMissingInstance(c => c.Fault()));

			Initially(When(Created)
				.Then(ctx => Console.WriteLine("Created"))
				.Publish(ctx => new StartFastOperation() { CorrelationId = ctx.Data.RequestId })
				.TransitionTo(OperationStarted));

			During(OperationStarted,
				When(Completed)
					.Then(ctx => Console.WriteLine("Completed"))
					.TransitionTo(Final));
		}

		public Event<Create> Created { get; private set; }

		public Event<FastOperationCompleted> Completed { get; private set; }

		public State OperationStarted { get; private set; }
	}
}
