/*
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

// Added logic to reason whether to ask hinge motor for effort or just use same effort as sent from JointCommandWriter
// 2020, Tyler Stephans (tbs5111@psu.edu)

using RosSharp.Urdf;
using UnityEngine;
using Joint = UnityEngine.Joint;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(Joint)), RequireComponent(typeof(UrdfJoint))]
    public class JointStateReader : MonoBehaviour
    {
        private UrdfJoint urdfJoint;
        private JointCommandWriter jointCommandWriter;
        private bool haveCommandWriter = false;

        private void Start()
        {
            urdfJoint = GetComponent<UrdfJoint>();
            haveCommandWriter = TryGetComponent<JointCommandWriter>(out jointCommandWriter);
        }

        public void Read(out string name, out float position, out float velocity, out float effort)
        {
            name = urdfJoint.JointName;
            position = urdfJoint.GetPosition();
            velocity = urdfJoint.GetVelocity();
            // Only ask for command effort from ROS if JointCommandWriter is an attached component and if it is currently writer commands
            if (haveCommandWriter && jointCommandWriter.isWritingCommands)
                effort = urdfJoint.GetCmdEffort();
            else
                effort = urdfJoint.GetEffort();
        }
    }
}
