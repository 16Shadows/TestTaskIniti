using System.Collections.Concurrent;

namespace Simulation.TrafficLights
{
    /// <summary>
    /// Реализация канала связи между светофорами в виде просто медиатора.
    /// </summary>
    public class TrafficLightsMediator : ITrafficLightChannel
    {
        /// <summary>
        /// Список всех зарегистрированных светофоров.
        /// </summary>
        private readonly ConcurrentDictionary<int, ITrafficLight> _TrafficLights = new();

        /// <summary>
        /// Отправить сообщение светофору с заданным id.
        /// </summary>
        /// <param name="receiverID">ID светофора, которому нужно доставить сообщение.</param>
        /// <param name="message">Сообщение.</param>
        /// <exception cref="KeyNotFoundException">Бросается, если светофора с таким ID нет в системе.</exception>
        public void DispatchMessage(int receiverID, TrafficLightMessageBase message)
        {
            if (!_TrafficLights.TryGetValue(receiverID, out ITrafficLight? light) || light == null)
                throw new KeyNotFoundException($"Invalid traffic light id - {receiverID}");

            light.EnqueueMessage(message);
        }

        /// <summary>
        /// Добавить светофор в роли приёмника в канал связи.
        /// </summary>
        /// <param name="trafficLight">Светофор, который нужно добавить.</param>
        /// <exception cref="ArgumentException">Бросается, если светофор с таким ID уже добавлен.</exception>
        public void Attach(ITrafficLight trafficLight)
        {
            if (!_TrafficLights.TryAdd(trafficLight.ID, trafficLight))
                throw new ArgumentException($"A traffic light with the same ID ({trafficLight.ID}) has already been added", nameof(trafficLight));
        }
    }
}
