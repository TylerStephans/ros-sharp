// Write velocity or effort commands to joints at every physics step. If a new command is not
// received, then continue to send the last command sent for commandTimer seconds.
// 2020, Tyler Stephans (tbs5111@psu.edu)

using RosSharp.Urdf;
using UnityEngine;
using Joint = UnityEngine.Joint;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(Joint)), RequireComponent(typeof(UrdfJoint))]
    public class JointCommandWriter : MonoBehaviour
    {
        private UrdfJoint urdfJoint;

        [Tooltip("How long to wait for a new command before the old command is no longer written to the joint.")]
        public float commandTimer = 0.5f;

        private float timeLastCommand;
        private float newCommand;
        private float curCommand;
        private bool newCommandReceived;
        public bool isWritingCommands { get; private set; }

        private JointCommandSubscriber.availableCommands commandToWrite;

        private void Start()
        {
            timeLastCommand = -commandTimer - 1.0f; // Make sure a command is not written until at least the first one is received.
            urdfJoint = GetComponent<UrdfJoint>();
        }

        //Update the command to the joint on every physics iteration.
        private void FixedUpdate()
        {
            if (newCommandReceived)
            {
                curCommand = newCommand;
                timeLastCommand = Time.fixedTime;
                newCommandReceived = false;
                isWritingCommands = true;
                WriteUpdate();
            }
            else if (Time.fixedTime - timeLastCommand < commandTimer)
            {
                // If no new command was recieved, then continue writing previous command until commandTimer seconds have passed.
                isWritingCommands = true;
                WriteUpdate();
            }
            else
                isWritingCommands = false;
        }

        private void WriteUpdate()
        {
            if (commandToWrite.Equals(JointCommandSubscriber.availableCommands.velocity))
            {
                urdfJoint.UpdateJointCmdVel(curCommand);
            }
            else if (commandToWrite.Equals(JointCommandSubscriber.availableCommands.effort))
            {
                urdfJoint.UpdateJointCmdEff(curCommand);
            }

        }

        public void Write(float command, JointCommandSubscriber.availableCommands selectedCommand)
        {
            newCommand = command;
            commandToWrite = selectedCommand;
            newCommandReceived = true;
            Debug.Log("Writing command for " + urdfJoint.JointName);
        }
    }
}