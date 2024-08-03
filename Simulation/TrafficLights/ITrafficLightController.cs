namespace Simulation.TrafficLights
{
	/// <summary>
	/// Интерфейс компонента управления текущим светофором.
	/// </summary>
	public interface ITrafficLightController
	{
		/// <summary>
        /// Указывает, разрешает ли этот светофор движение по управляемой им дороге.
        /// </summary>
		bool CanBePassed { set; }
		/// <summary>
        /// ID этого светофора.
        /// </summary>
		int ID { get; }

		/// <summary>
		/// Отправить сообщение светофору немедленно.
		/// </summary>
		/// <param name="targetId">ID светофора, которому нужно отправить сообщение.</param>
		/// <param name="message">Сообщение, которое нужно отправить светофору.</param>
		void DispatchMessage(int targetId, TrafficLightMessageBase message);
		
		/// <summary>
		/// Отправить сообщение светофору с задержкой.
		/// </summary>
		/// <param name="targetId">ID светофора, которому нужно отправить сообщение.</param>
		/// <param name="message">Сообщение, которое нужно отправить светофору.</param>
		/// <param name="delayMs">Задержка (в мс) перед отправкой сообщения.</param>
		void DispatchMessage(int targetId, TrafficLightMessageBase message, int delayMs);
	}
}
