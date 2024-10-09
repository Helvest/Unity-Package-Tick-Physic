using System;
using System.Collections.Generic;
using UnityEngine;

namespace TickPhysics
{
	public abstract class TickSystem : ITickSystem
	{

		#region Fields

		public virtual bool IsPhysicUpdated { get; set; } = true;

		public virtual double TimeAtSimulation { get; protected set; } = 0;

		public virtual double NormalTime { get; protected set; } = 0;

		public virtual double FixedTime { get; protected set; } = 0;

		public virtual uint FixedFrameCount { get; protected set; } = 0;

		public virtual float ExtraDeltaTime { get; protected set; } = 0;

		protected virtual bool IsInUpdateLoop { get; set; } = false;

		protected virtual int LoopIndex { get; set; } = 0;

		#endregion

		#region Events

		public event Action EventReadInput;
		public event Action EventUpdatePhysic;
		public event Action EventProcessInput;
		public event Action EventUpdateGraphic;

		#endregion

		#region IPhysicObject

		protected readonly List<IPhysicsObject> physicObjects = new List<IPhysicsObject>(256);

		public virtual void Add(params IPhysicsObject[] physicObjectsToAdd)
		{
			foreach (var item in physicObjectsToAdd)
			{
				if (!physicObjects.Contains(item))
				{
					physicObjects.Add(item);
				}
			}
		}

		public virtual void Remove(params IPhysicsObject[] physicObjectsToRemove)
		{
			foreach (var item in physicObjectsToRemove)
			{
				int index = physicObjects.IndexOf(item);
				physicObjects.RemoveAt(index);

				if (IsInUpdateLoop && index < LoopIndex)
				{
					LoopIndex--;
				}
			}
		}

		#endregion

		#region Tick

		public virtual void Tick(double time, double deltaTime, double fixedDeltaTime)
		{
			ReadInput();

			if (!IsPhysicUpdated)
			{
				return;
			}

			TimeAtSimulation = time;

			NormalTime += deltaTime;

			PhysicTick(fixedDeltaTime);

			CalculateExtraDeltaTime();

			// Here you can access the transforms state right after the simulation, if needed...	
			UpdateGraphic();

			//IsInUpdateLoop = false;
		}

		protected virtual void PhysicTick(double fixedDeltaTime)
		{
			if (fixedDeltaTime <= 0)
			{
				return;
			}

			// Catch up with the game time.
			// Advance the physics simulation by portions of fixedDeltaTime
			while (NormalTime >= FixedTime + fixedDeltaTime)
			{
				FixedFrameCount++;

				FixedTime += fixedDeltaTime;

				//A custom FixedUpdate
				UpdatePhysic();

				//Advance the simulation, this will also call OnTrigger and OnCollider
				SimulatePhysic(fixedDeltaTime);

				//Prepare inputs for next physic frame
				ProcessInput();
			}
		}

		protected virtual void CalculateExtraDeltaTime()
		{
			ExtraDeltaTime = (float)(NormalTime - FixedTime);
		}

		#endregion

		#region Custom Updates

		protected virtual void ReadInput()
		{
			EventReadInput?.Invoke();
		}

		protected virtual void UpdatePhysic()
		{
			IsInUpdateLoop = true;

			for (LoopIndex = 0; LoopIndex < physicObjects.Count; LoopIndex++)
			{
				physicObjects[LoopIndex].UpdatePhysics();
			}

			IsInUpdateLoop = false;

			EventUpdatePhysic?.Invoke();
		}

		protected virtual void ProcessInput()
		{
			EventProcessInput?.Invoke();
		}

		protected abstract void SimulatePhysic(double fixedDeltaTime);

		protected virtual void UpdateGraphic()
		{
			IsInUpdateLoop = true;

			for (LoopIndex = 0; LoopIndex < physicObjects.Count; LoopIndex++)
			{
				physicObjects[LoopIndex].UpdateGraphics();
			}

			IsInUpdateLoop = false;

			EventUpdateGraphic?.Invoke();
		}

		#endregion

	}
}
