namespace Simulation.Roads
{
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
	}
}
