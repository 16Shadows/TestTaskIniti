namespace Simulation.TrafficLights
{
    /// <summary>
    /// Базовый класс сообщений между светофорами.
    /// </summary>
    public class TrafficLightMessageBase
    {
        /// <summary>
        /// ID светофора, отправившего это сообщение.
        /// </summary>
        public int SenderID { get; }

        public TrafficLightMessageBase(int senderID)
        {
            SenderID = senderID;
        }
    }
}
