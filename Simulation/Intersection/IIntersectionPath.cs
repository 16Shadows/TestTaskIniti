namespace Simulation.Intersection
{
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
	}
}