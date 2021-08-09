using System;

namespace TickPhysics
{
	public interface ITickManager
	{

		#region Variables

		public bool IsPhysicUpdated { get; set; }

		public bool AutoUpdate { get; set; }

		public bool AutoSimulation { get; set; }

		#endregion

		#region Event

		public event Action EventReadInput;

		public event Action EventUpdatePhysic;

		public event Action EventProcessInput;

		public event Action EventUpdateGraphic;

		#endregion

		#region IPhysicObject

		public void Add(params IPhysicsObject[] physicObject);

		public void Remove(params IPhysicsObject[] physicObject);

		#endregion


		#region Update

		public void Tick(float deltaTime, float fixedDeltaTime);


		#endregion

	}
}
