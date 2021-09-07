using UnityEngine;

namespace TickPhysics
{
	public class TickManager3D : TickManager
	{

		#region AutoSimulation

		public override bool AutoSimulation
		{
			get
			{
				return _autoSimulation;
			}

			set
			{
				_autoSimulation = value;

				Physics.autoSimulation = _autoSimulation;
			}
		}

		#endregion

		#region SimulatePhysic

		protected override void SimulatePhysic(double fixedDeltaTime)
		{
			Physics.Simulate((float)fixedDeltaTime);
		}

		#endregion

	}

}
