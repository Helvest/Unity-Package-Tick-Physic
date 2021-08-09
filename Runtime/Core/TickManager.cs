using System;
using System.Collections.Generic;
using UnityEngine;

namespace TickPhysics
{

	public abstract class TickManager : MonoBehaviour, ITickManager
	{

		#region Variables

		[SerializeField]
		private bool _isPhysicUpdated = true;

		public virtual bool IsPhysicUpdated
		{
			get => _isPhysicUpdated;
			set => _isPhysicUpdated = value;
		}

		[SerializeField]
		private bool _autoUpdate = false;

		public virtual bool AutoUpdate
		{
			get => _autoUpdate;
			set => _autoUpdate = value;
		}

		[SerializeField]
		protected bool _autoSimulation = false;

		public virtual bool AutoSimulation
		{
			get => _autoSimulation;
			set => _autoSimulation = value;
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

		protected virtual void Update()
		{
			if (AutoUpdate)
			{
				Tick(Time.time, Time.deltaTime, Time.fixedDeltaTime);
			}
		}

		#endregion

		#region Tick

		public void Tick(float time, float deltaTime, float fixedDeltaTime)
		{
			Tick((double)time, deltaTime, fixedDeltaTime);
		}

		public virtual void Tick(double time, double deltaTime, double fixedDeltaTime)
		{
			ReadInput();

			if (IsPhysicUpdated)
			{
				TimeAtSimulation = time;

				NormalTime += deltaTime;

				PhysicTick(deltaTime, fixedDeltaTime);

				// Here you can access the transforms state right after the simulation, if needed...	
				UpdateGraphic();
			}
		}

		protected virtual void PhysicTick(double deltaTime, double fixedDeltaTime)
		{
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
