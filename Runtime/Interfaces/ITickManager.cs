using System;

namespace TickPhysics
{
	public interface ITickManager
	{

		#region Variables

		bool IsPhysicUpdated { get; set; }

		bool AutoUpdate { get; set; }

		bool AutoSimulation { get; set; }

		float ExtraDeltaTime { get; }

		#endregion

		#region Event

		event Action EventReadInput;

		event Action EventUpdatePhysic;

		event Action EventProcessInput;

		event Action EventUpdateGraphic;

		#endregion

		#region IPhysicObject

		void Add(params IPhysicsObject[] physicObject);

		void Remove(params IPhysicsObject[] physicObject);

		#endregion

		#region Update

		void Tick(double time, double deltaTime, double fixedDeltaTime);

		#endregion

	}
}
