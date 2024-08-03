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

		enum StateTransitionStep
		{
			InOldState,
			DisablingTrafficLights,
			EnablingTrafficLights,
			StateApplied
		}

		/// <summary>
		/// Текущее состояние.
		/// </summary>
		private TrafficLightsState? _ActiveState;
		private StateTransitionStep _TransitionStep;
		private List<int>? _AwaitingTrafficLights;

		/// <summary>
		/// Выбрать новое активное состояние.
		/// </summary>
		/// <returns>Возвращает true, если новое состояние было выбрано и начат переход к нему. False, если предыдущий переход не завершён или новое состояние не было выбрано.</returns>
		private bool SelectNewState()
		{
			if (_TransitionStep != StateTransitionStep.StateApplied)
				return false;

			TrafficLightsState state = _States.MaxBy(state => state.EvaluateStateScore(_QueueSizes), _ScoreComparer)!;
			if (state != _ActiveState)
			{
				_ActiveState = state;
				_TransitionStep = StateTransitionStep.InOldState;
				TransitionToActiveState();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Выполнить часть перехода к новому активному состоянию
		/// </summary>
		/// <returns>True - всё в норме, продолжаем переходить. False - процесс завершён или что-то пошло не так, нужно выбирать новое состояние.</returns>
		private bool TransitionToActiveState()
		{
			//ActiveState стал null (safeguard, не должно происходить в норме.
			if (_ActiveState == null)
			{
				_TransitionStep = StateTransitionStep.StateApplied;
				return false;
			}

			if (_TransitionStep == StateTransitionStep.InOldState)
			{
				_AwaitingTrafficLights = _ActiveState.DisableTrafficLights(_Controller);
				_TransitionStep = StateTransitionStep.DisablingTrafficLights;
			}
			else if (_TransitionStep == StateTransitionStep.DisablingTrafficLights)
			{
				if (_AwaitingTrafficLights == null)
					return false;
				else if (_AwaitingTrafficLights.Count == 0)
				{
					_AwaitingTrafficLights = _ActiveState.EnableTrafficLights(_Controller);
					_TransitionStep = StateTransitionStep.EnablingTrafficLights;
				}
			}
			else if (_TransitionStep == StateTransitionStep.EnablingTrafficLights)
			{
				if (_AwaitingTrafficLights == null)
					return false;
				else if (_AwaitingTrafficLights.Count == 0)
				{
					_TransitionStep = StateTransitionStep.StateApplied;
					return false;
				}
			}
			else
				return false;

			return true;
		}

		public void OnMessage(TrafficLightMessageBase message)
		{
			if (message is TrafficLightQueueSizeChangedMessage sizeChanged)
			{
				_QueueSizes[sizeChanged.SenderID] = sizeChanged.NewSize;
			}
			else if (message is TrafficLightOnStateSetMessage stateSet && _AwaitingTrafficLights != null)
			{
				int index = _AwaitingTrafficLights.IndexOf(stateSet.SenderID);
				if (index > -1)
				{
					_AwaitingTrafficLights[index] = _AwaitingTrafficLights[^1];
					_AwaitingTrafficLights.RemoveAt(_AwaitingTrafficLights.Count - 1);

					if (!TransitionToActiveState())
						_Controller.DispatchMessage(_Controller.ID, _UpdateStateMessage, _UpdateStateInterval);
				}
			}
			//Для однородности реализации изменение состояния этого светофора тоже производится сообщением, отправляемым самому себе.
			else if (message is TrafficLightSetStateMessage setState && message.SenderID == _Controller.ID)
			{
				_Controller.CanBePassed = setState.CanBePassed;
				_Controller.DispatchMessage(_Controller.ID, new TrafficLightOnStateSetMessage(_Controller.ID));
			}
			//Обновляем состояние FSM
			else if (message is UpdateTrafficLightsStateMessage && message.SenderID == _Controller.ID)
			{
				if (!SelectNewState())
					_Controller.DispatchMessage(_Controller.ID, _UpdateStateMessage, _UpdateStateInterval);
			}	
		}

		public void OnQueueSizeChanged(int newSize)
		{
			_QueueSizes[_Controller.ID] = newSize;
		}

		public void OnStart()
		{
			_ActiveState = null;
			_TransitionStep = StateTransitionStep.StateApplied;
			SelectNewState();
		}

		public void OnStop()
		{
			_QueueSizes.Clear();
		}
	}
}
