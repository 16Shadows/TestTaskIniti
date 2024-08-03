using System.Collections.Concurrent;
using Simulation.Cameras;
using Simulation.Core;

namespace Simulation.TrafficLights
{
    /// <summary>
    /// Реализация светофора на основе <see cref="SimulationEntityBase"/>
    /// </summary>
    public class TrafficLight : SimulationEntityBase, ITrafficLightController, ITrafficLight
    {
        /// <summary>
        /// Очередь сообщений.
        /// </summary>
        private readonly ConcurrentQueue<TrafficLightMessageBase> _Messages = new ConcurrentQueue<TrafficLightMessageBase>();
        /// <summary>
        /// Компонент, реализующий логику управления светофором.
        /// </summary>
        private readonly ITrafficLightBrain _Brain;
        /// <summary>
        /// Компонент камеры
        /// </summary>
        private readonly ITrafficLightCamera _Camera;
        /// <summary>
        /// Компонент канала для обмена сообщениями между светофорами.
        /// </summary>
        private readonly ITrafficLightChannel _Channel;

        /// <summary>
        /// Объект для синхронизации состояния светофора.
        /// </summary>
        protected readonly object _TrafficLightStateSyncRoot = new object();

        private bool _CanBePassed;
		public bool CanBePassed
        {
            get
            {
                lock (_TrafficLightStateSyncRoot)
                {
                    return _CanBePassed;
                }
            }
            set
            {
                lock (_TrafficLightStateSyncRoot)
                {
                    _CanBePassed = value;
                }
                PassabilityChanged?.Invoke(this);
            }
        }

        public event TrafficLightPassabilityChanged? PassabilityChanged;

		public int ID { get; }

        /// <summary>
        /// Создать светофор с заданными компонентами.
        /// </summary>
        /// <param name="id">ID этого светофора.</param>
        /// <param name="camera">Камера этого светофора.</param>
        /// <param name="channel">Канал связи этого светофора.</param>
        /// <param name="brainFactory">Метод-фабрика для создания компонента логики управления этим светофором. Получает компонент для управления этим светофором в качестве аргумента.</param>
        /// <exception cref="ArgumentNullException">Бросается, если <paramref name="camera"/>, <paramref name="channel"/>, <paramref name="brainFactory"/> имеет значение <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Бросается, если <paramref name="brainFactory"/> возвращает <c>null</c>.</exception>
		public TrafficLight(int id, ITrafficLightCamera camera, ITrafficLightChannel channel, Func<ITrafficLightController, ITrafficLightBrain> brainFactory)
        {
            ID = id;
            _Camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _Brain = (brainFactory ?? throw new ArgumentNullException(nameof(brainFactory)))(this) ?? throw new ArgumentException("Traffic light brain factory returned null.", nameof(brainFactory));
            _Channel = channel ?? throw new ArgumentNullException(nameof(channel));

            _Camera.QueueSizeChanged += OnQueueSizeChanged;
        }

		public void EnqueueMessage(TrafficLightMessageBase message)
        {
            _Messages.Enqueue(message);
            TryAwakeSimulation();
        }

		public void DispatchMessage(int targetId, TrafficLightMessageBase message)
		{
			_Channel.DispatchMessage(targetId, message);
		}

		public void DispatchMessage(int targetId, TrafficLightMessageBase message, int delayMs)
		{
			Task.Run(async () =>
            {
                await Task.Delay(delayMs);
                _Channel.DispatchMessage(targetId, message);
            });
		}

        /// <summary>
        /// Объект для синхронизации изменений размера очереди
        /// </summary>
        private readonly object _QueueSyncRoot = new object();
        /// <summary>
        /// Флаг, указывающий, изменился ли размер очереди с предыдущего SimulationStep.
        /// </summary>
        private bool _QueueSizeChanged = false;
        

        private void OnQueueSizeChanged(ITrafficLightCamera queue)
        {
            if (queue == _Camera)
            {
                //Перекладываем обработку изменения очереди на SimulationStep, чтобы методы _Brain всегда вызывались синхронно из одного потока.

                lock (_QueueSyncRoot)
                    _QueueSizeChanged = true;

                TryAwakeSimulation();
            }
        }

		protected override void SimulationStep()
		{
			while (_Messages.TryDequeue(out var mes))
                _Brain.OnMessage(mes);
                
            lock (_QueueSyncRoot)
            {
                if (_QueueSizeChanged)
                {
                    _Brain.OnQueueSizeChanged(_Camera.QueueSize);
                    _QueueSizeChanged = false;
                }
            }
		}
	}
}