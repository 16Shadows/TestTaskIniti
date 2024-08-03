using Simulation.TrafficLights;

namespace Algorithm
{
	/// <summary>
	/// Компонент управления главным светофором, определяющим состояние всех светофоров на перекрёстке.
	/// </summary>
	public class TrafficLightMasterBrain : ITrafficLightBrain
	{
		private class UpdateTrafficLightsStateMessage : TrafficLightMessageBase
		{
			public UpdateTrafficLightsStateMessage(int senderID) : base(senderID)
			{
			}
		}

		private readonly UpdateTrafficLightsStateMessage _UpdateStateMessage;
		private readonly int _UpdateStateInterval;

		/// <summary>
		/// Компаратор счетов состояний
		/// </summary>
		private static readonly TrafficLightsState.StateScoreComparer _ScoreComparer = new();

		/// <summary>
		/// Последние известные размеры очередей светофоров по ID.
		/// </summary>
		private readonly Dictionary<int, int> _QueueSizes = new Dictionary<int, int>();
		/// <summary>
		/// Набор конфигураций светофоров на перекрёстке.
		/// </summary>
		private readonly TrafficLightsState[] _States;
		/// <summary>
		/// Контроллер этого светофора.
		/// </summary>
		private readonly ITrafficLightController _Controller;

		/// <summary>
		/// Создать компонент управления главным светофором.
		/// </summary>
		/// <param name="controller">Контроллер этого светофора.</param>
		/// <param name="states">Набор конфигураций светофоров на перекрёстке.</param>
		/// <param name="updateIntervalMs">Частота обновления состояния.</param>
		/// <exception cref="TrafficLightMasterBrain">Бросается, если <paramref name="controller"/> или <paramref name="states"/> имеет значение <c>null</c>.</exception>
		public TrafficLightMasterBrain(ITrafficLightController controller, IEnumerable<TrafficLightsState> states, int updateIntervalMs)
		{
			_States = (states ?? throw new ArgumentNullException(nameof(states))).ToArray();

			if (_States.Length < 1)
				throw new ArgumentException("Master traffic light must have at least one state.", nameof(states));

			_Controller = controller ?? throw new ArgumentNullException(nameof(controller));
			_UpdateStateMessage = new UpdateTrafficLightsStateMessage(controller.ID);
			_UpdateStateInterval = updateIntervalMs;
		}

		/// <summary>
		/// Текущее состояние.
		/// </summary>
		private TrafficLightsState? _ActiveState;

		/// <summary>
		/// Выбрать новое активное состояние.
		/// </summary>
		private void UpdateState()
		{
			TrafficLightsState state = _States.MaxBy(state => state.EvaluateStateScore(_QueueSizes), _ScoreComparer)!;
			if (state != _ActiveState)
			{
				state.ApplyState(_Controller);
				_ActiveState = state;
			}
			_Controller.DispatchMessage(_Controller.ID, _UpdateStateMessage, _UpdateStateInterval);
		}

		public void OnMessage(TrafficLightMessageBase message)
		{
			if (message is TrafficLightQueueSizeChangedMessage sizeChanged)
			{
				_QueueSizes[sizeChanged.SenderID] = sizeChanged.NewSize;
			}
			//Для однородности реализации изменение состояния этого светофора тоже производится сообщением, отправляемым самому себе.
			else if (message is TrafficLightSetStateMessage setState && message.SenderID == _Controller.ID)
			{
				_Controller.CanBePassed = setState.CanBePassed;
			}
			//Обновляем состояние FSM
			else if (message is UpdateTrafficLightsStateMessage updateState && message.SenderID == _Controller.ID)
				UpdateState();
		}

		public void OnQueueSizeChanged(int newSize)
		{
			_QueueSizes[_Controller.ID] = newSize;
		}

		public void OnStart()
		{
			UpdateState();
		}

		public void OnStop()
		{
			_QueueSizes.Clear();
		}
	}
}
