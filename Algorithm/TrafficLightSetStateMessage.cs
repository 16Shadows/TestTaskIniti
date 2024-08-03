using Simulation.TrafficLights;

namespace Algorithm
{
	/// <summary>
	/// Сообщение о необходимости изменить состояние светофора.
	/// </summary>
	public class TrafficLightSetStateMessage : TrafficLightMessageBase
	{
		/// <summary>
		/// Должен ли светофор пропускать сущности в новом состоянии.
		/// </summary>
		public bool CanBePassed { get; }

		public TrafficLightSetStateMessage(int senderID, bool canBePassed) : base(senderID)
		{
			CanBePassed = canBePassed;
		}
	}
}
