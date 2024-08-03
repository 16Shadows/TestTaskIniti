using Simulation.TrafficLights;

namespace Algorithm
{
	/// <summary>
	/// Одно из возможных состояний FSM перекрёстка.
	/// </summary>
	public class TrafficLightsState
	{
		/// <summary>
		/// Класс для сравнения оценок состояний.
		/// Выбираем сначала по кол-во активных светофоров с непустыми очередями, затем - по сумме размеров очередей.
		/// </summary>
		public class StateScoreComparer : IComparer<(int, int)>
		{
			public int Compare((int, int) x, (int, int) y)
			{
				int firstComp = x.Item1.CompareTo(y.Item1);
				return firstComp == 0 ? x.Item2.CompareTo(y.Item2) : firstComp;
			}
		}

		private readonly int[] _EnableTrafficLights;
		private readonly int[] _DisableTrafficLights;

		/// <summary>
		/// Создать состояние
		/// </summary>
		/// <param name="enableTrafficLights">ID светофоров, пропускающих машины в этом состоянии.</param>
		/// <param name="disableTrafficLights">ID светофоров, блокирующих машины в этом состоянии.</param>
		public TrafficLightsState(IEnumerable<int> enableTrafficLights, IEnumerable<int> disableTrafficLights)
		{
			_EnableTrafficLights = enableTrafficLights.ToArray();
			_DisableTrafficLights = disableTrafficLights.ToArray();
		}

		/// <summary>
		/// Подсчитать оценку этого состояния.
		/// Оценка состояния состоит из двух компонентов.
		/// Первый - количество активных светофоров этого состояния, у которых размер очереди больше 0.
		/// Второй - сумма размеров очередей активных светофоров.
		/// </summary>
		/// <param name="trafficLightQueues">Размеры очередей светофоров по ID.</param>
		/// <returns></returns>
		public (int, int) EvaluateStateScore(IReadOnlyDictionary<int, int> trafficLightQueues)
		{
			int queuesNotEmpty = 0;
			int totalQueuesSize = 0;
			foreach (int id in _EnableTrafficLights)
			{
				if (!trafficLightQueues.TryGetValue(id, out int value))
					continue;
				
				if (value > 0)
					queuesNotEmpty++;
				totalQueuesSize += value;
			}
			return (queuesNotEmpty, totalQueuesSize);
		}

		/// <summary>
		/// Применить это состояние ко всем светофорам путём рассылки сообщений.
		/// </summary>
		/// <param name="controller">Контроллер для отправки сообщений.</param>
		public void ApplyState(ITrafficLightController controller)
		{
			foreach (int id in _EnableTrafficLights)
				controller.DispatchMessage(id, new TrafficLightSetStateMessage(controller.ID, true));

			foreach (int id in _DisableTrafficLights)
				controller.DispatchMessage(id, new TrafficLightSetStateMessage(controller.ID, false));
		}
	}
}
