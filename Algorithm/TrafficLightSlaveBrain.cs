using Simulation.TrafficLights;

namespace Algorithm
{
	/// <summary>
	/// Компонент управления подчинённым светофором, который получает команды от главного светофора и отправляет ему обновления.
	/// </summary>
	public class TrafficLightSlaveBrain : ITrafficLightBrain
	{
		/// <summary>
		/// ID главного светофора.
		/// </summary>
		private readonly int _MasterID;
		/// <summary>
		/// Контроллер этого светофора
		/// </summary>
		private readonly ITrafficLightController _Controller;

		/// <summary>
		/// Создать компонент управления подчинённым светофором.
		/// </summary>
		/// <param name="controller">Контроллер этого светофора.</param>
		/// <param name="masterID">ID главного светофора.</param>
		public TrafficLightSlaveBrain(ITrafficLightController controller, int masterID)
		{
			_MasterID = masterID;
			_Controller = controller;
		}

		public void OnMessage(TrafficLightMessageBase message)
		{
			if (message.SenderID != _MasterID)
				return;

			if (message is TrafficLightSetStateMessage setState)
				_Controller.CanBePassed = setState.CanBePassed;
		}

		public void OnQueueSizeChanged(int newSize)
		{
			_Controller.DispatchMessage(_MasterID, new TrafficLightQueueSizeChangedMessage(_Controller.ID, newSize));
		}
	}
}
