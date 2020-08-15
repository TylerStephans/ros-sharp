/*
© Siemens AG, 2017-2019
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

// Modified so that the publisher could be run at a desired frequency in *simulation* time
// 2020, Tyler Stephans (tbs5111@psu.edu)

using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class JointStatePublisher : UnityPublisher<MessageTypes.Sensor.JointState>
    {
        public List<JointStateReader> JointStateReaders;
        public string FrameId = "Unity";

        [UnityEngine.Tooltip("Will publish every FixedUpdate if 0 or too large.")]
        public float publishFrequency = 0f;
        private float nStepsWait;   // Number of physics steps to wait each time before publishing
        private float stepsRemaining;      // Number of steps remaining before publishing. If < 1 then publish

        private MessageTypes.Sensor.JointState message;    
        
        protected override void Start()
        {
            if (1f / publishFrequency < UnityEngine.Time.fixedDeltaTime)    //If desired frequency is too large, then set to publish every fixed update instead.
            {
                UnityEngine.Debug.LogWarning("Specified publishing frequency is too large. Will publish on every FixedUpdate().");
                nStepsWait = 1f;
            }
            else if (publishFrequency > 0)
                nStepsWait = 1f / UnityEngine.Time.fixedDeltaTime / publishFrequency;
            else // If the published frequency is negative or zero
                nStepsWait = 1f;
            stepsRemaining = nStepsWait;

            base.Start();
            InitializeMessage();
        }
        
        private void FixedUpdate()
        {
            //Update message approximately at desired frequency
            stepsRemaining--;
            if (stepsRemaining < 1)
            {
                stepsRemaining += nStepsWait;
                UpdateMessage();
            }
            
        }
        
        private void InitializeMessage()
        {
            int jointStateLength = JointStateReaders.Count;
            message = new MessageTypes.Sensor.JointState
            {
                header = new MessageTypes.Std.Header { frame_id = FrameId },
                name = new string[jointStateLength],
                position = new double[jointStateLength],
                velocity = new double[jointStateLength],
                effort = new double[jointStateLength]
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            for (int i = 0; i < JointStateReaders.Count; i++)
                UpdateJointState(i);

            Publish(message);
        }

        private void UpdateJointState(int i)
        {

            JointStateReaders[i].Read(
                out message.name[i],
                out float position,
                out float velocity,
                out float effort);

            message.position[i] = position;
            message.velocity[i] = velocity;
            message.effort[i] = effort;
        }


    }
}
