namespace Simulation.Intersection
{
	/// <summary>
	/// Делегат событий <see cref="IIntersectionPath.PathPassed"/>.
	/// </summary>
	/// <param name="path">Путь перекрёстка, который был пройден.</param>
	public delegate void OnPathPassed(IIntersectionPath path);

	/// <summary>
	/// Интерфейс пути через перекрёсток.
	/// </summary>
	public interface IIntersectionPath
	{
		/// <summary>
		/// Если ли ожидающие пройти по этому пути сущности и позволяют ли это сделать элементы управления перекрёстком.
		/// </summary>
		bool ExpectsPassage { get; }
		
		/// <summary>
		/// Произойдёт ли столкновение на этом пути перекрёстка в текущей конфигурации.
		/// </summary>
		bool HasCollision { get; }

		/// <summary>
		/// Выполнить прохождение перекрёстка по этому пути.
		/// </summary>
		void Pass();

		/// <summary>
		/// Вызывается, когда что-то успешно проходит по этому пути.
		/// </summary>
		event OnPathPassed PathPassed;
	}
}