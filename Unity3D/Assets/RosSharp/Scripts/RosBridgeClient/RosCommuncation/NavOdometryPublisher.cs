﻿/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Added allocation free alternatives
// UoK , 2019, Odysseas Doumas (od79@kent.ac.uk / odydoum@gmail.com)
// Modified to publish actual odometry
// Tyler Stephans (tbs5111@psu.edu / stephanst223@gmail.com)

using System;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class NavOdometryPublisher : UnityPublisher<MessageTypes.Nav.Odometry>
    {
        public Transform PublishedFrameTransform;
        public string FrameId = "Unity";
        public Transform PublishedChildFrameTransform;
        public string ChildFrameId = "base_link";

        public Rigidbody PublishedRigidbodyTransfrorm;

        private MessageTypes.Nav.Odometry message;

        private float previousRealTime;
        private Vector3 previousPosition = Vector3.zero;
        private Quaternion previousRotation = Quaternion.identity;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Nav.Odometry()
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                },
                child_frame_id = ChildFrameId
            };

            message.pose = new MessageTypes.Geometry.PoseWithCovariance();
            message.twist = new MessageTypes.Geometry.TwistWithCovariance();
            /*message = new MessageTypes.Geometry.Twist();
            message.linear = new MessageTypes.Geometry.Vector3();
            message.angular = new MessageTypes.Geometry.Vector3();
            */
        }

        private void UpdateMessage()
        {
            message.header.Update();
            // Update pose
            GetGeometryPoint(PublishedRigidbodyTransfrorm.position.Unity2Ros(), message.pose.pose.position);
            GetGeometryQuaternion(PublishedRigidbodyTransfrorm.rotation.Unity2Ros(), message.pose.pose.orientation);
            SetIdealCovariance(message.pose.covariance);

            // Update twist
            float deltaTime = Time.realtimeSinceStartup - previousRealTime;
            ////float deltaTime = Time.deltaTime;

            // Changed velocity calculations to be based on Unity recommended Rigidbody information due to noise from manual calculation.
            ////Vector3 linearVelocity = (PublishedChildFrameTransform.position - previousPosition) / deltaTime;
            ////Vector3 angularVelocity = (PublishedChildFrameTransform.rotation.eulerAngles - previousRotation.eulerAngles) / deltaTime;

            Vector3 linearVelocity = PublishedRigidbodyTransfrorm.velocity;
            Vector3 angularVelocity = PublishedRigidbodyTransfrorm.angularVelocity;

            linearVelocity = PublishedChildFrameTransform.InverseTransformVector(linearVelocity);
            angularVelocity = PublishedChildFrameTransform.InverseTransformVector(angularVelocity);

            message.twist.twist.linear = GetGeometryVector3(linearVelocity.Unity2Ros());
            message.twist.twist.angular = GetGeometryVector3(-angularVelocity.Unity2Ros());
            SetIdealCovariance(message.twist.covariance);

            previousRealTime = Time.realtimeSinceStartup;
            ////previousPosition = PublishedChildFrameTransform.position;
            ////previousRotation = PublishedChildFrameTransform.rotation;

            if (Math.Abs(message.twist.twist.angular.x) > 1000 && PublishedFrameTransform.name == "ball")
                Debug.Log("x angular maxed at " + message.twist.twist.angular.x.ToString());

            Publish(message);
        }

        private static void GetGeometryPoint(Vector3 position, MessageTypes.Geometry.Point geometryPoint)
        {
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
        }

        private static void GetGeometryQuaternion(Quaternion quaternion, MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            geometryQuaternion.x = quaternion.x;
            geometryQuaternion.y = quaternion.y;
            geometryQuaternion.z = quaternion.z;
            geometryQuaternion.w = quaternion.w;
        }

        private static MessageTypes.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            MessageTypes.Geometry.Vector3 geometryVector3 = new MessageTypes.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }

        private static void SetIdealCovariance(double[] covariance)
        {
            double ideal_val = 0.00001;
            covariance = new double[36];
            for (int i = 0; i < 6; i++)
                covariance[6 * i + i] = ideal_val;
        }

    }
}
