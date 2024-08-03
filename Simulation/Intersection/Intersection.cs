using Simulation.Core;

namespace Simulation.Intersection
{
	/// <summary>
	/// Делегат события столкновения на перекрёстке.
	/// </summary>
	/// <param name="intersection">Перекрёсток, на котором произошло столкновение.</param>
	/// <param name="path">Путь, на котором произошло столкновение.</param>
	public delegate void OnIntersectionCollision(Intersection intersection, IIntersectionPath path);

	/// <summary>
	/// Реализация перекрёстка.
	/// </summary>
	public class Intersection : SimulationEntityBase
	{
		private readonly IIntersectionPath[] _Paths;
		private readonly int _PollIntervalMs;

		/// <summary>
		/// Событие столкновения на перекрёстке.
		/// </summary>
		public event OnIntersectionCollision? OnIntersectionCollision;

		/// <summary>
		/// Создать перекрёсток с заданными путями и временем прохождения.
		/// </summary>
		/// <param name="pollIntervalMs">Время прохождения перекрёстка (частота проверки путей).</param>
		/// <param name="paths">Пути перекрёстка.</param>
		/// <exception cref="ArgumentOutOfRangeException">Бросается, если <paramref name="pollIntervalMs"/> меньше 1 мс.</exception>
		public Intersection(int pollIntervalMs, IEnumerable<IIntersectionPath> paths)
		{
			if (pollIntervalMs < 1)
				throw new ArgumentOutOfRangeException(nameof(pollIntervalMs), "Polling interval must be greater than 0 ms.");
				
			_PollIntervalMs = pollIntervalMs;
			_Paths = paths.ToArray() ?? throw new ArgumentNullException(nameof(paths));
		}

		protected override void SimulationStep()
		{
			Thread.Sleep(_PollIntervalMs);
			foreach (var path in _Paths)
			{
				if (!path.ExpectsPassage)
					continue;

				if (path.HasCollision)
				{
					OnIntersectionCollision?.Invoke(this, path);
					break;
				}

				path.Pass();
			}
			TryAwakeSimulation();
		}
	}
}
