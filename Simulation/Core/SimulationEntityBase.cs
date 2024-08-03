namespace Simulation.Core
{
    /// <summary>
    /// Реализация <see cref="ISimulationEntity"/> в виде симуляции на отдельном потоке (<see cref="Thread"/>). 
    /// </summary>
	public abstract class SimulationEntityBase : ISimulationEntity
	{
        /// <summary>
        /// Запущена ли симуляция на этом объекте
        /// </summary>
        private bool _IsSimulationRunning;
        /// <summary>
        /// Объект для синхронизации работы с этой сущностью в целом
        /// </summary>
        private readonly object _SimulationSyncRoot = new object();
        /// <summary>
        /// Объект для синхронизации изнутри потока симуляции
        /// </summary>
        private readonly object _SimulationThreadSyncRoot = new object();
        /// <summary>
        /// Поток симуляции
        /// </summary>
        private Thread? _SimulationThread;
        /// <summary>
        /// CV для пробуждения потока симуляции
        /// </summary>
        private AutoResetEvent? _SimulationThreadCV;

		public bool IsSimulationRunning
        {
            get
            {
                lock (_SimulationSyncRoot)
                    return _IsSimulationRunning;
            }
        }

        public void StartSimulation()
        {
            lock (_SimulationSyncRoot)
            {
                if (_IsSimulationRunning || _SimulationThread != null || _SimulationThreadCV != null)
                    throw new InvalidOperationException("The simulation on this entity is already running.");

                try
                {
                    _SimulationThreadCV = new AutoResetEvent(false);
                    
                    SetUpSimulation();
                    
                    _IsSimulationRunning = true;

                    _SimulationThread = new Thread(SimulationThread);
                    _SimulationThread.Start();
                }
                catch
                {
                    _IsSimulationRunning = false;
                    
                    _SimulationThreadCV?.Set();
                    _SimulationThread?.Join();

                    _SimulationThreadCV = null;
                    _SimulationThread = null;

                    throw;
                }
            }
        }

        public Task StopSimulation()
        {
            lock (_SimulationSyncRoot)
            {
                if (!_IsSimulationRunning)
                    throw new InvalidOperationException("The simulation on this entity is not running.");
                else if (_SimulationThreadCV == null || _SimulationThread == null)
                    throw new InvalidOperationException("The simulation on this entity is in inconsistent state.");

                lock (_SimulationThreadSyncRoot)
                    _IsSimulationRunning = false;

                _SimulationThreadCV.Set();
                
                //Завершаем симуляцию асинхронно, чтобы избежать deadlock'а, если симуляция будет завершена изнутри потока симуляции.
                return Task.Run(() =>
                {
                    lock (_SimulationSyncRoot)
                    {
                        _SimulationThread.Join();

                        _SimulationThreadCV = null;
                        _SimulationThread = null;

                        TearDownSimulation();
                    }
                });
            }
        }

        /// <summary>
        /// Поток симуляции
        /// </summary>
        private void SimulationThread()
        {
            while (true)
            {
                lock (_SimulationThreadSyncRoot)
                {
                    if (!_IsSimulationRunning)
                        return;
                }

                int sleepFor = SimulationStep();

                if (sleepFor > 0)
                    _SimulationThreadCV?.WaitOne(sleepFor);
                else if (sleepFor < 0)
                    _SimulationThreadCV?.WaitOne();
            }
        }

        /// <summary>
        /// Шаг симуляции, после которого поток симуляции отправляется в режим ожидания до пробуждения через <see cref="TryAwakeSimulation"/> или на указанное время.
        /// </summary>
        /// <returns>Возвращает - время сна в мс. Если время сна < 0, то поток спит до пробуждения.</returns>
        protected abstract int SimulationStep();
        
        /// <summary>
        /// Вызывается перед началом симуляции для инициализации состояния сущности.
        /// </summary>
        protected virtual void SetUpSimulation() {}

        /// <summary>
        /// Вызывается после завершения симуляции для деконструкции состояния сущности.
        /// </summary>
        protected virtual void TearDownSimulation() {}

        /// <summary>
        /// Пробудить поток симуляции для совершения следующего шага симуляции (<see cref="SimulationStep"/>).
        /// Если поток симуляции уже выполняет шаг симуляции, то он по его завершению он немедленно выполнит ещё один шаг симуляции.
        /// </summary>
        protected void TryAwakeSimulation()
        {
            lock (_SimulationThreadSyncRoot)
            {
                if (!_IsSimulationRunning)
                    return;

                _SimulationThreadCV?.Set();
            }
        }
    }
}
