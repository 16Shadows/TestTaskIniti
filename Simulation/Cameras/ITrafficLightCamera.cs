namespace Simulation.Cameras
{
    /// <summary>
    /// Делагат события изменения размера очереди перед камерой (<see cref="ITrafficLightCamera"/>)
    /// </summary>
    /// <param name="sender"></param>
    public delegate void TrafficLightCameraQueueSizeChanged(ITrafficLightCamera sender);

    /// <summary>
    /// Интерфейс камеры светофора.
    /// </summary>
    public interface ITrafficLightCamera
    {
        /// <summary>
        /// Текущий зафиксированный размер очереди перед камерой.
        /// </summary>
        int QueueSize { get; }

        /// <summary>
        /// Событие изменения размера очереди перед камерой.
        /// </summary>
        event TrafficLightCameraQueueSizeChanged QueueSizeChanged;
    }
}