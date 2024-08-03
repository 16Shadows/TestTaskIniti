namespace Simulation.Core
{
    /// <summary>
    /// Класс для синхронизации запуска/остановки симуляции множества сущностей.
    /// </summary>
    public class SimulationOrchestrator
    {
        /// <summary>
        /// Сущности в этой симуляции.
        /// </summary>
        private HashSet<ISimulationEntity> _Entities = new HashSet<ISimulationEntity>();
        /// <summary>
        /// Объект для синхронизации симуляции.
        /// </summary>
        private readonly object _SyncRoot = new object();

        /// <summary>
        /// Запущена ли уже симуляция.
        /// </summary>
        public bool IsSimulationRunning { get; private set; }

        /// <summary>
        /// Добавить сущность в симуляцию.
        /// </summary>
        /// <param name="entity">Сущность, которую нужно добавить.</param>
        /// <exception cref="InvalidOperationException">Бросается, если симуляция уже запущена.</exception>
        public void AddEntity(ISimulationEntity entity)
        {
            lock (_SyncRoot)
            {
                if (IsSimulationRunning)
                    throw new InvalidOperationException("Cannot add an entity to a running simulation");

                _Entities.Add(entity);
            }
        }

        /// <summary>
        /// Запустить симуляцию на всех объектах.
        /// </summary>
        /// <exception cref="InvalidOperationException">Бросается, если симуляция уже запущена.</exception>
        /// <exception cref="Exception">Бросается, если было брошено исключение при запуске симуляции одной из сущностей. Содержит брошенное сущностью исключение.</exception>
        public void StartSimulation()
        {
            lock(_SyncRoot)
            {
				if (IsSimulationRunning)
                    throw new InvalidOperationException("The simulation is already running.");
            }
            
            try
            {
                foreach (var entity in _Entities)
                    entity.StartSimulation();
                
                lock (_SyncRoot)
                    IsSimulationRunning = true;
            }
            catch (Exception e)
            {
                List<Task> terminations = new List<Task>();
                foreach (var entity in _Entities)
                {
                    try
                    {
                        if (entity.IsSimulationRunning)
                            terminations.Add(entity.StopSimulation());
                    }
                    catch { }
                }
                Task.WaitAll(terminations.ToArray());
                throw new Exception("Failed to start simulation", e);
            }
        }

        /// <summary>
        /// Остановить симуляцию на всех объектах симуляции.
        /// </summary>
        /// <returns>Асинхронное завершение</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AggregateException"></exception>
        public Task StopSimulation()
        {
            lock (_SyncRoot)
            {
                if (!IsSimulationRunning)
                    throw new InvalidOperationException("The simulation is not running.");
            }

            List<Exception> innerExceptions = new List<Exception>();
            Task[] terminations = new Task[_Entities.Count];
            int i = 0;
            foreach (var entity in _Entities)
            {
                try
                {
                    terminations[i++] = entity.StopSimulation();
                }
                catch (Exception e)
                {
                    innerExceptions.Add(e);
                }
            }
            return Task.WhenAll(terminations).ContinueWith((t) =>
            {
                lock (_SyncRoot)
                    IsSimulationRunning = false;
                    
                if (innerExceptions.Count > 0)
                    throw new AggregateException("One or more exceptions has occured while stopping the simulation", innerExceptions);
            });
        }
    }
}
