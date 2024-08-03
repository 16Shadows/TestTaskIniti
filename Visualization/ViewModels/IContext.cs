using System;

namespace Visualization.ViewModels
{
    /// <summary>
    /// Контекст вью-модели.
    /// </summary>
    interface IContext
    {
        /// <summary>
        /// Выполнить действие в контексте вью-модели на корректном потоке.
        /// </summary>
        /// <param name="action">Действие.</param>
        void Invoke(Action action);
    }
}
