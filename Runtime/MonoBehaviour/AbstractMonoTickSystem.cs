using System;
using UnityEngine;

namespace TickPhysics
{
	public abstract class AbstractMonoTickSystem<T> : MonoBehaviour, ITickSystem where T : ITickSystem
	{

		#region Fields

		[field: SerializeField]
		public T TickSystem { get; private set; }

		[SerializeField]
		private bool _isPhysicUpdated = true;

		public bool IsPhysicUpdated
		{
			get => TickSystem.IsPhysicUpdated;
			set => TickSystem.IsPhysicUpdated = _isPhysicUpdated = value;
		}

		public float ExtraDeltaTime => TickSystem.ExtraDeltaTime;

		public double TimeAtSimulation => TickSystem.TimeAtSimulation;

		public double NormalTime => TickSystem.NormalTime;

		public double FixedTime => TickSystem.FixedTime;

		public uint FixedFrameCount => TickSystem.FixedFrameCount;

		#endregion

		#region Event

		public event Action EventReadInput
		{
			add
			{
				TickSystem.EventReadInput += value;
			}

			remove
			{
				TickSystem.EventReadInput -= value;
			}
		}

		public event Action EventUpdatePhysic
		{
			add
			{
				TickSystem.EventUpdatePhysic += value;
			}

			remove
			{
				TickSystem.EventUpdatePhysic -= value;
			}
		}

		public event Action EventProcessInput
		{
			add
			{
				TickSystem.EventProcessInput += value;
			}

			remove
			{
				TickSystem.EventProcessInput -= value;
			}
		}

		public event Action EventUpdateGraphic
		{
			add
			{
				TickSystem.EventUpdateGraphic += value;
			}

			remove
			{
				TickSystem.EventUpdateGraphic -= value;
			}
		}

		#endregion

		#region OnEnable

		protected virtual void OnEnable()
		{
			IsPhysicUpdated = _isPhysicUpdated;
		}

		#endregion

		#region LateUpdate

#if UNITY_EDITOR

		protected virtual void LateUpdate()
		{
			if (IsPhysicUpdated != _isPhysicUpdated)
			{
				IsPhysicUpdated = _isPhysicUpdated;
			}
		}

#endif

		#endregion

		#region Tick

		public virtual void Tick(double time, double deltaTime, double fixedDeltaTime)
		{
			TickSystem.Tick(time, deltaTime, fixedDeltaTime);
		}

		#endregion

		#region IPhysicObject

		public void Add(params IPhysicsObject[] physicObject) => TickSystem.Add(physicObject);

		public void Remove(params IPhysicsObject[] physicObject) => TickSystem.Remove(physicObject);

		#endregion

	}
}
