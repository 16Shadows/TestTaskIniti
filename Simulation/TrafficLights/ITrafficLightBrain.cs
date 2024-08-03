namespace Simulation.TrafficLights
{
	/// <summary>
	/// Интерфейс класса, реализующего логику управления светофором.
	/// </summary>
	public interface ITrafficLightBrain
	{
		/// <summary>
		/// Вызывается, когда необходимо обработать следующее сообщение из очереди сообщений светофора.
		/// </summary>
		/// <param name="message">Сообщение, которое необходимо обработать.</param>
		void OnMessage(TrafficLightMessageBase message);

		/// <summary>
		/// Вызывается, когда размер очереди этого светофора изменяется.
		/// </summary>
		/// <param name="newSize">Новый размер очереди светофора.</param>
		void OnQueueSizeChanged(int newSize);
	}
}
