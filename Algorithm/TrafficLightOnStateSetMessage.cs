using Simulation.TrafficLights;

namespace Algorithm
{
	internal class TrafficLightOnStateSetMessage : TrafficLightMessageBase
	{
		public TrafficLightOnStateSetMessage(int senderID) : base(senderID)
		{
		}
	}
}
