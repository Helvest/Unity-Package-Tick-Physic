using System;

namespace TickPhysics
{
	public interface ITickSystem
	{

		#region Fields

		bool IsPhysicUpdated { get; set; }

		double TimeAtSimulation { get; }

		double NormalTime { get; }

		double FixedTime { get; }

		uint FixedFrameCount { get; }

		float ExtraDeltaTime { get; }

		#endregion

		#region Event

		event Action EventReadInput;

		event Action EventUpdatePhysic;

		event Action EventProcessInput;

		event Action EventUpdateGraphic;

		#endregion

		#region IPhysicObject

		void Add(params IPhysicsObject[] physicObjectsToAdd);

		void Remove(params IPhysicsObject[] physicObjectsToRemove);

		#endregion

		#region Tick

		void Tick(double time, double deltaTime, double fixedDeltaTime);

		#endregion

	}
}
