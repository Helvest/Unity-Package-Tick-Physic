using UnityEngine;

namespace TickPhysics
{
	public class TickManager2D : TickManager
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

				Physics2D.simulationMode =_autoSimulation ? SimulationMode2D.FixedUpdate : SimulationMode2D.Script;
			}
		}

		#endregion

		#region SimulatePhysic

		protected override void SimulatePhysic(double fixedDeltaTime)
		{
			Physics2D.Simulate((float)fixedDeltaTime);
		}

		#endregion

	}

}
