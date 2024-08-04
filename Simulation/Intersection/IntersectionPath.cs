namespace Simulation.Intersection
{
	/// <summary>
	/// Реализация пути перекрёстка.
	/// </summary>
	public class IntersectionPath : IIntersectionPath
	{
		private static readonly IIntersectionPath[] _NoPaths = new IIntersectionPath[0];

		private readonly IIntersectionEntrance _PathEntrance;
		private IIntersectionPath[] _IntersectingPaths;

		/// <summary>
		/// Создать путь перекрёстка.
		/// </summary>
		/// <param name="entrance">Вход этого пути.</param>
		/// <param name="crossEntrances">
		/// Входы, пересекающиеся с этим перекрёстком.
		/// Если у хотя бы одного из них <see cref="IIntersectionEntrance.ExpectsPassage"/> имеет значение <c>true</c>, то <see cref="HasCollision"/> будет иметь значение <c>true</c>.
		/// </param>
		/// <exception cref="ArgumentNullException">Бросается, если <paramref name="entrance"/> имеет значение <c>null</c>.</exception>
		public IntersectionPath(IIntersectionEntrance entrance)
		{
			_PathEntrance = entrance ?? throw new ArgumentNullException(nameof(entrance));
			_IntersectingPaths = _NoPaths;
		}

		public void SetIntersectingPaths(params IIntersectionPath[] crossPaths) => _IntersectingPaths = crossPaths ?? _NoPaths;
		public void SetIntersectingPaths(IEnumerable<IIntersectionPath> crossPaths) => _IntersectingPaths = crossPaths?.ToArray() ?? _NoPaths;

		public bool ExpectsPassage => _PathEntrance.ExpectsPassage;

		public bool HasCollision => _PathEntrance.ExpectsPassage && _IntersectingPaths.Any(x => x.ExpectsPassage);

		public void Pass()
		{
			_PathEntrance.SimulatePassage();
		}
	}
}
