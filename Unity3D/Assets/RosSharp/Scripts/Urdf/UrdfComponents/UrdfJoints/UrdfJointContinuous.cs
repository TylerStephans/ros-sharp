/*
© Siemens AG, 2018-2019
Author: Suzannah Smith (suzannah.smith@siemens.com)

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

// Added support for applying velocity and effort commands, and replaced references to the angle property
// of the hinge joint with a manually calculated corrected angle.
// Added ability to return effort command instead of motor force.
//  *WARNING* JointCommandWriter.writingCommands should be checked before using GetCmdEffort.
// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;

namespace RosSharp.Urdf
{
    public class UrdfJointContinuous : UrdfJoint
    {
        private float rosEffort;

        public static UrdfJoint Create(GameObject linkObject)
        {
            UrdfJointContinuous urdfJoint = linkObject.AddComponent<UrdfJointContinuous>();
            urdfJoint._jointType = JointTypes.Continuous;
            urdfJoint.UnityJoint = linkObject.AddComponent<HingeJoint>();
            urdfJoint.UnityJoint.autoConfigureConnectedAnchor = true;
            // add custom angle calculation
            linkObject.AddComponent<CorrectHingeAngle>();

            return urdfJoint;
        }

        #region Runtime

        public override float GetPosition()
        {
            //return -((HingeJoint)UnityJoint).angle * Mathf.Deg2Rad;
            return -GetComponent<CorrectHingeAngle>().Angle() * Mathf.Deg2Rad;
        }
        public override float GetVelocity()
        {
            //return -((HingeJoint)UnityJoint).velocity * Mathf.Deg2Rad;
            return -GetComponent<CorrectHingeAngle>().Velocity() * Mathf.Deg2Rad;
        }
        public override float GetEffort()
        {
            return -((HingeJoint)UnityJoint).motor.force;
        }
        public override float GetCmdEffort()
        {
            return rosEffort;
        }

        protected override void OnUpdateJointState(float deltaState)
        {
            Quaternion rot = Quaternion.AngleAxis(-deltaState * Mathf.Rad2Deg, UnityJoint.axis);
            transform.rotation = transform.rotation * rot;
            //((HingeJoint)UnityJoint).gameObject.GetComponent<Rigidbody>().MoveRotation(transform.rotation * rot);
        }
        protected override void OnUpdateJointCmdEff(float newCommand)
        {
            rosEffort = newCommand;
            var worldAxis = transform.TransformVector(((HingeJoint)UnityJoint).axis);
            var effort = newCommand * worldAxis.normalized;

            ((HingeJoint)UnityJoint).gameObject.GetComponent<Rigidbody>().AddTorque(-effort);
            if (((HingeJoint)UnityJoint).connectedBody != null)
                ((HingeJoint)UnityJoint).connectedBody.AddTorque(effort);

        }
        protected override void OnUpdateJointCmdVel(float newState)
        {
            JointMotor jointMotor = ((HingeJoint)UnityJoint).motor;
            jointMotor.targetVelocity = newState;
            ((HingeJoint)UnityJoint).motor = jointMotor;
        }
        #endregion

        protected override void ImportJointData(Joint joint)
        {
            UnityJoint.axis = (joint.axis != null) ? GetAxis(joint.axis) : GetDefaultAxis();

            if (joint.dynamics != null)
                ((HingeJoint)UnityJoint).spring = GetJointSpring(joint.dynamics);
        }

        protected override Joint ExportSpecificJointData(Joint joint)
        {
            joint.axis = GetAxisData(UnityJoint.axis);
            joint.dynamics = new Joint.Dynamics(
                ((HingeJoint)UnityJoint).spring.damper, 
                ((HingeJoint)UnityJoint).spring.spring);
            
            return joint;
        }
    }
}

