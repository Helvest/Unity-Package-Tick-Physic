using System;
using System.Collections.Generic;
using UnityEngine;

namespace TickPhysics
{

	public abstract class TickManager : MonoBehaviour, ITickManager
	{

		#region Variables

		[SerializeField]
		protected bool _autoSimulation = false;

		public abstract bool AutoSimulation { get; set; }

		[SerializeField]
		protected bool isPhysicUpdated = true;

		public virtual bool IsPhysicUpdated
		{
			get
			{
				return isPhysicUpdated;
			}

			set
			{
				if (isPhysicUpdated == value)
				{
					return;
				}

				isPhysicUpdated = value;

				if (isPhysicUpdated)
				{
					FixedTime = Time.time;
				}
			}
		}

		public double TimeAtSimulation { get; protected set; } = 0;

		public double NormalTime { get; protected set; } = 0;

		public double FixedTime { get; protected set; } = 0;

		public uint FixedFrameCount { get; protected set; } = 0;

		public float ExtraDeltaTime { get; protected set; } = 0;

		#endregion

		#region Events

		public event Action EventReadInput;
		public event Action EventUpdatePhysic;
		public event Action EventProcessInput;
		public event Action EventUpdateGraphic;

		#endregion

		#region OnEnable

		protected virtual void OnEnable()
		{
			FixedTime = Time.time;

			AutoSimulation = _autoSimulation;
		}

		#endregion

		#region IPhysicObject

		protected List<IPhysicsObject> _physicObjects = new List<IPhysicsObject>(256);

		public virtual void Add(params IPhysicsObject[] physicObjects)
		{
			foreach (var item in physicObjects)
			{
				if (!_physicObjects.Contains(item))
				{
					_physicObjects.Add(item);
				}
			}
		}

		public virtual void Remove(params IPhysicsObject[] physicObjects)
		{
			foreach (var item in physicObjects)
			{
				_physicObjects.Remove(item);
			}
		}

		#endregion

		#region Update

		public virtual bool AutoUpdate { get; set; } = false;

		protected virtual void Update()
		{
			if (AutoUpdate)
			{
				Tick(Time.deltaTime, Time.fixedDeltaTime);
			}
		}

		#endregion

		#region Tick

		public virtual void Tick(float deltaTime, float fixedDeltaTime)
		{
			ReadInput();

			if (isPhysicUpdated)
			{
				TimeAtSimulation = Time.time;

				NormalTime += deltaTime;

				PhysicTick(deltaTime, fixedDeltaTime);

				// Here you can access the transforms state right after the simulation, if needed...	
				UpdateGraphic();
			}
		}

		protected virtual void PhysicTick(double deltaTime, double fixedDeltaTime)
		{
			// Catch up with the game time.
			// Advance the physics simulation in portions of Time.fixedDeltaTime
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

			ExtraDeltaTime = (float)(NormalTime - FixedTime + deltaTime);
		}

		#endregion

		#region Custom Updates

		protected virtual void ReadInput()
		{
			EventReadInput?.Invoke();
		}

		protected virtual void UpdatePhysic()
		{
			foreach (var physicObject in _physicObjects)
			{
				physicObject.UpdatePhysics();
			}

			EventUpdatePhysic?.Invoke();
		}

		protected virtual void ProcessInput()
		{
			EventProcessInput?.Invoke();
		}

		protected abstract void SimulatePhysic(double fixedDeltaTime);

		protected virtual void UpdateGraphic()
		{
			foreach (var physicObject in _physicObjects)
			{
				physicObject.UpdateGraphics();
			}

			EventUpdateGraphic?.Invoke();
		}

		#endregion

	}

}
