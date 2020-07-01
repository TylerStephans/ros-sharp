// This script is largely based on the C# script for wheel-motor in Unity
// in Appendix C:
// https://www.diva-portal.org/smash/record.jsf?pid=diva2%3A1334348&dswid=1540
// And the diff_drive_controller provided by the ros_controllers ROS repository:
// https://github.com/ros-controls/ros_controllers.git
using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class TwistSubscriberMotorDriver : UnitySubscriber<MessageTypes.Geometry.Twist>
    {
        public float WheelRadius = 0.1651F; // [m]
        public float WheelSeparation = 0.5708F; // [m]
        public float WheelSeparationMultiplier = 1.875F; // default to 1
        public float WheelRadiusMultiplier = 1.0F; // default to 1
        public List<HingeJoint> WheelLeft;
        public List<HingeJoint> WheelRight;

        private float ws;
        private float wr;

        private float vel_left; // [deg/s]
        private float vel_right; // [deg/s]
        private bool is_message_recieved = false;
        //private List<JointMotor> joint_motor_left;
        //private List<JointMotor> joint_motor_right;
        private float cmd_lin; // [m/s]
        private float cmd_ang; // [rad/s]

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < WheelLeft.Count; i++)
            {
                WheelLeft[i].useMotor = true;
                //joint_motor_left[i] =  WheelLeft[i].motor;
            }
            for (int i = 0; i < WheelRight.Count; i++)
            {
                WheelRight[i].useMotor = true;
                //joint_motor_right[i] = WheelRight[i].motor;
            }
            wr = WheelRadius * WheelRadiusMultiplier;
            ws = WheelSeparation * WheelSeparationMultiplier;
        }

        // Update is called once per frame
        private void Update()
        {
            if (is_message_recieved)
                Process();
        }
        private void Process()
        {
            for (int i = 0; i < WheelLeft.Count; i++)
            {
                JointMotor cur_motor = WheelLeft[i].motor;
                cur_motor.targetVelocity = vel_left;
                WheelLeft[i].motor = cur_motor;
            }
            for (int i = 0; i < WheelRight.Count; i++)
            {
                JointMotor cur_motor = WheelRight[i].motor;
                cur_motor.targetVelocity = vel_right;
                WheelRight[i].motor = cur_motor;
            }
            is_message_recieved = false;
        }
        private static Vector3 ToVector3(MessageTypes.Geometry.Vector3 geometryVector3)
        {
            return new Vector3((float)geometryVector3.x, (float)geometryVector3.y, (float)geometryVector3.z);
        }
        protected override void ReceiveMessage(MessageTypes.Geometry.Twist message)
        {
            cmd_lin = ToVector3(message.linear).Ros2Unity().z;
            cmd_ang = ToVector3(message.angular).Ros2Unity().y;
            vel_left = 180 / Mathf.PI * (cmd_lin - cmd_ang * ws / 2.0F) / wr;
            vel_right = 180 / Mathf.PI * (cmd_lin + cmd_ang * ws / 2.0F) / wr;
            is_message_recieved = true;
        }
    }
}

