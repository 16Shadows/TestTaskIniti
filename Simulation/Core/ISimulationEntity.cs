namespace Simulation.Core
{
    /// <summary>
    /// Интерфейс сущности симуляции
    /// </summary>
    public interface ISimulationEntity
    {
        /// <summary>
        /// Запущена ли симуляция на этой сущности
        /// </summary>
        bool IsSimulationRunning { get; }

        /// <summary>
        /// Запустить симуляцию на этой сущности.
        /// </summary>
        /// <exception cref="InvalidOperationException">Должно быть брошено, если симуляция уже запущена.</exception>
        void StartSimulation();
        /// <summary>
        /// Остановить симуляцию на этой сущности
        /// </summary>
        /// <returns>Асинхронное ожидание завершения симуляции. Если симуляция будет завершена изнутри симуляции, ожидание этого завершения может привести к deadlock'у.</returns>
        /// <exception cref="InvalidOperationException">Должно быть брошено, если симуляция не запущена.</exception>
        Task StopSimulation();
    }
}