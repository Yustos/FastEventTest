using Automatonymous;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker
{
	public class SagaInstance : SagaStateMachineInstance, IVersionedSaga
	{
		public ObjectId Id { get; set; }

		public Guid CorrelationId { get; set; }

		public int Version { get; set; }

		public string CurrentState { get; set; }
	}
}
