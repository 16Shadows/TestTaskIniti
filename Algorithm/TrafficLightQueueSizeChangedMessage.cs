using Simulation.TrafficLights;

namespace Algorithm
{
	/// <summary>
	/// Сообщение об изменении размера очереди светофора.
	/// </summary>
	public class TrafficLightQueueSizeChangedMessage : TrafficLightMessageBase
	{
		/// <summary>
		/// Новый размер очереди светофора.
		/// </summary>
		public int NewSize { get; }

		public TrafficLightQueueSizeChangedMessage(int senderID, int newSize) : base(senderID)
		{
			NewSize = newSize;
		}
	}
}
