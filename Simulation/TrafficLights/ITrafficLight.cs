namespace Simulation.TrafficLights
{
    public delegate void TrafficLightPassabilityChanged(ITrafficLight trafficLight);

    /// <summary>
    /// Интерфейс светофора.
    /// </summary>
    public interface ITrafficLight
    {
        /// <summary>
        /// Добавляет новое сообщение в очередь сообщений этого светофора.
        /// </summary>
        /// <param name="message">Сообщение, которое необходимо добавить.</param>
        void EnqueueMessage(TrafficLightMessageBase message);

        /// <summary>
        /// Указывает, разрешает ли этот светофор движение по управляемой им дороге.
        /// </summary>
        bool CanBePassed { get; }

        /// <summary>
        /// ID этого светофора.
        /// </summary>
        public int ID { get; }

        /// <summary>
		/// Событие об изменении состояния светофора для визуализации.
		/// </summary>
        event TrafficLightPassabilityChanged PassabilityChanged;
    }
}
