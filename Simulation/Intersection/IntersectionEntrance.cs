using Simulation.Roads;
using Simulation.TrafficLights;

namespace Simulation.Intersection
{
	/// <summary>
	/// Реализация входа перекрёстка.
	/// </summary>
	public class IntersectionEntrance : IIntersectionEntrance
	{
		public bool ExpectsPassage
		{
			get
			{
				return _Road.QueueSize > 0 && _TrafficLight.CanBePassed;
			}
		}

		/// <summary>
		/// Дорога, с которой машины входят на перекрёсток.
		/// </summary>
		private readonly IRoad _Road;

		/// <summary>
		/// Светофор, который контролирует этот вход.
		/// </summary>
		private readonly ITrafficLight _TrafficLight;

		/// <summary>
		/// Создать вход на перекрёсток.
		/// </summary>
		/// <param name="road">Дорога, с которой машины входят на перекрёсток.</param>
		/// <param name="trafficLight">Светофор, управляющий этим входом.</param>
		/// <exception cref="ArgumentNullException">Бросается, если <paramref name="road"/> или <paramref name="trafficLight"/> имеет значение <c>null</c>.</exception>
		public IntersectionEntrance(IRoad road, ITrafficLight trafficLight)
		{
			_Road = road ?? throw new ArgumentNullException(nameof(road));
			_TrafficLight = trafficLight ?? throw new ArgumentNullException(nameof(trafficLight));
		}

		public void SimulatePassage()
		{
			if (!ExpectsPassage)
				throw new InvalidOperationException("This intersection entrance doesn't expect passage!");

			_Road.PopQueue();
		}
	}
}
