namespace Simulation.Intersection
{
	/// <summary>
	/// Реализация пути перекрёстка.
	/// </summary>
	public class IntersectionPath : IIntersectionPath
	{
		private readonly IIntersectionEntrance _PathEntrance;
		private readonly IIntersectionEntrance[] _CrossEntrances;

		/// <summary>
		/// Создать путь перекрёстка.
		/// </summary>
		/// <param name="entrance">Вход этого пути.</param>
		/// <param name="crossEntrances">
		/// Входы, пересекающиеся с этим перекрёстком.
		/// Если у хотя бы одного из них <see cref="IIntersectionEntrance.ExpectsPassage"/> имеет значение <c>true</c>, то <see cref="HasCollision"/> будет иметь значение <c>true</c>.
		/// </param>
		/// <exception cref="ArgumentNullException">Бросается, если <paramref name="entrance"/> имеет значение <c>null</c>.</exception>
		public IntersectionPath(IIntersectionEntrance entrance, IEnumerable<IIntersectionEntrance> crossEntrances)
		{
			_PathEntrance = entrance ?? throw new ArgumentNullException(nameof(entrance));
			_CrossEntrances = crossEntrances?.ToArray() ?? new IIntersectionEntrance[0];
		}

		public bool ExpectsPassage => _PathEntrance.ExpectsPassage;

		public bool HasCollision => _PathEntrance.ExpectsPassage && _CrossEntrances.Any(x => x.ExpectsPassage);

		public void Pass()
		{
			_PathEntrance.SimulatePassage();
		}
	}
}
