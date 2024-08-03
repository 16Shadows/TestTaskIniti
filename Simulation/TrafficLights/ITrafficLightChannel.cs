namespace Simulation.TrafficLights
{
	/// <summary>
	/// Интерфейс механизма обмена сообщениями между светофорами.
	/// </summary>
	public interface ITrafficLightChannel
	{
		/// <summary>
		/// Отправить сообщение светофору.
		/// </summary>
		/// <param name="receiverID">ID светофора, которому нужно отправить сообщение.</param>
		/// <param name="message">Сообщение, которое нужно отправить светофору.</param>
		void DispatchMessage(int receiverID, TrafficLightMessageBase message);
	}
}
