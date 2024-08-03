namespace Simulation.Roads
{
	public delegate void RoadQueueSizeChanged(IRoad road);

	/// <summary>
	/// Интерфейс дороги (очереди на подходе к перекрёстку)
	/// </summary>
	public interface IRoad
	{
		/// <summary>
		/// Текущий размер очереди.
		/// </summary>
		int QueueSize { get; }
		
		/// <summary>
		/// Убрать одну сущность из очереди
		/// </summary>
		void PopQueue();

		/// <summary>
		/// Событие об изменении размера очереди для визуализации.
		/// </summary>
		event RoadQueueSizeChanged QueueSizeChanged;
	}
}
