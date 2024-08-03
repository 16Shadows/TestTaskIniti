namespace Simulation.Intersection
{
	/// <summary>
	/// Интерфейс входа перекрёстка - дороги, на которой собираются объекты (машины/пешеходы).
	/// </summary>
	public interface IIntersectionEntrance
	{
		/// <summary>
		/// Есть ли у этого входа желающие пересечь перекрёсток и позволяют ли элементы управления это сделать.
		/// </summary>
		bool ExpectsPassage { get; }

		/// <summary>
		/// Симулировать пересечение перекрёстка.
		/// </summary>
		void SimulatePassage();
	}
}
