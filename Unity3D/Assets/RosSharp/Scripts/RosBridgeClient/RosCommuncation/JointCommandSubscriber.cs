// Modified JointStateSubscriber to subscribe to joint commands instead
// 2020, Tyler Stephans (tbs5111@psu.edu)

using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class JointCommandSubscriber : UnitySubscriber<MessageTypes.Sensor.JointState>
    {
        // Only offer velocity and effort for now...
        public enum availableCommands
        {
            velocity,
            effort
        }
        public availableCommands commandToWrite = availableCommands.effort;
        //public static bool recievedMessage = false;

        public List<string> JointNames;
        public List<JointCommandWriter> JointCommandWriters;

        protected override void ReceiveMessage(MessageTypes.Sensor.JointState message)
        {
            int index;
            for (int i = 0; i < message.name.Length; i++)
            {
                index = JointNames.IndexOf(message.name[i]);
                if (index != -1)
                {
                    if (commandToWrite.Equals(availableCommands.effort))
                        JointCommandWriters[index].Write((float)message.effort[i], commandToWrite);
                    else if (commandToWrite.Equals(availableCommands.velocity))
                        JointCommandWriters[index].Write((float)message.velocity[i], commandToWrite);
                }

            }
            // Possible work around to update physics after message recieved... does not currently work
            /*
            if (!UnityEngine.Physics.autoSimulation)
            {
                UnityEngine.Physics.Simulate(0.01f);
                gameObject.GetComponent<ClockPublisher>().UpdateClock();
                gameObject.GetComponent<JointStatePublisher>().UpdateMessage();
            }
            */
        }
    }
}