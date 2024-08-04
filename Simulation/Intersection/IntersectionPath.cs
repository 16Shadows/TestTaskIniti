using Simulation.Roads;
using Simulation.TrafficLights;

namespace Simulation.Intersection
{
	/// <summary>
	/// Реализация пути перекрёстка.
	/// </summary>
	public class IntersectionPath : IIntersectionPath
	{
		private static readonly IIntersectionPath[] _NoPaths = new IIntersectionPath[0];

		/// <summary>
		/// Дорога, с которой машины входят на перекрёсток.
		/// </summary>
		private readonly IRoad _Road;

		/// <summary>
		/// Светофор, который контролирует этот вход.
		/// </summary>
		private readonly ITrafficLight? _TrafficLight;

		/// <summary>
		/// Пересекающиеся с этим пути.
		/// </summary>
		private IIntersectionPath[] _IntersectingPaths;

		/// <summary>
		/// Создать путь перекрёстка.
		/// </summary>
		/// <param name="road">Вход этого пути.</param>
		/// <param name="trafficLight">Светофор, контролирующий этот путь (если какой-либо).</param>
		/// <exception cref="ArgumentNullException">Бросается, если <paramref name="entrance"/> имеет значение <c>null</c>.</exception>
		public IntersectionPath(IRoad road, ITrafficLight? trafficLight)
		{
			_Road = road ?? throw new ArgumentNullException(nameof(road));
			_TrafficLight = trafficLight;
			_IntersectingPaths = _NoPaths;
		}

		public void SetIntersectingPaths(params IIntersectionPath[] crossPaths) => _IntersectingPaths = crossPaths ?? _NoPaths;
		public void SetIntersectingPaths(IEnumerable<IIntersectionPath> crossPaths) => _IntersectingPaths = crossPaths?.ToArray() ?? _NoPaths;

		public bool ExpectsPassage => _Road.QueueSize > 0 && _TrafficLight?.CanBePassed != false;

		public bool HasCollision => ExpectsPassage && _IntersectingPaths.Any(x => x.ExpectsPassage);

		public void Pass()
		{
			lock (_Road)
			{
				if (!ExpectsPassage)
					throw new InvalidOperationException("Cannot pass a path that doesn't expect passage.");

				_Road.PopQueue();
			}
		}
	}
}
