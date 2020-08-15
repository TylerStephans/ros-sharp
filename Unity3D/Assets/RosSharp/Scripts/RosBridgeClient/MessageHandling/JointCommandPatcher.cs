// Modified joint state patcher to patch joint commands instead
// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;
using System.Collections.Generic;
using RosSharp.Urdf;

namespace RosSharp.RosBridgeClient
{
    public class JointCommandPatcher : MonoBehaviour
    {
        public UrdfRobot UrdfRobot;
        // I do not have a need to publish commands, so I'm removing this capability
        /*
        public void SetPublishJointCommand(bool publish)
        {
            if (publish)
            {
                JointCommandPublisher jointCommandPublisher = transform.AddComponentIfNotExists<JointCommandPublisher>();
                jointStatePublisher.JointStateReaders = new List<JointStateReader>();

                foreach (UrdfJoint urdfJoint in UrdfRobot.GetComponentsInChildren<UrdfJoint>())
                {
                    if (urdfJoint.JointType != UrdfJoint.JointTypes.Fixed)
                        jointStatePublisher.JointStateReaders.Add(urdfJoint.transform.AddComponentIfNotExists<JointStateReader>());
                }
            }
            else
            {
                GetComponent<JointStatePublisher>()?.JointStateReaders.Clear();

                foreach (JointStateReader reader in UrdfRobot.GetComponentsInChildren<JointStateReader>())
                    reader.transform.DestroyImmediateIfExists<JointStateReader>();
            }
        }
        */
        public void SetSubscribeJointCommands(bool subscribe)
        {
            if (subscribe)
            {
                JointCommandSubscriber jointCommandSubscriber = transform.AddComponentIfNotExists<JointCommandSubscriber>();
                jointCommandSubscriber.JointCommandWriters = new List<JointCommandWriter>();
                jointCommandSubscriber.JointNames = new List<string>();

                foreach (UrdfJoint urdfJoint in UrdfRobot.GetComponentsInChildren<UrdfJoint>())
                {
                    if (urdfJoint.JointType != UrdfJoint.JointTypes.Fixed)
                    {
                        jointCommandSubscriber.JointCommandWriters.Add(urdfJoint.transform.AddComponentIfNotExists<JointCommandWriter>());
                        jointCommandSubscriber.JointNames.Add(urdfJoint.JointName);
                    }
                }
            }
            else
            {
                GetComponent<JointCommandSubscriber>()?.JointCommandWriters.Clear();
                GetComponent<JointCommandSubscriber>()?.JointNames.Clear();

                foreach (JointCommandWriter writer in UrdfRobot.GetComponentsInChildren<JointCommandWriter>())
                    writer.transform.DestroyImmediateIfExists<JointCommandWriter>();
            }
        }
    }
}