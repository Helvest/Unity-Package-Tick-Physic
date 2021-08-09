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

				if (_autoSimulation)
				{
					Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
				}
				else
				{
					Physics2D.simulationMode = SimulationMode2D.Script;
				}
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
