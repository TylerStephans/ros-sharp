// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;
using UnityEditor;

namespace RosSharp.RosBridgeClient
{
    [CustomEditor(typeof(CorrectHingeAngle))]
    public class CorrectHingeAngleEditor : UnityEditor.Editor
    {
        private CorrectHingeAngle correctHingeAngle;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUtility.SetDirty(target); // Necessary so that correctHingeAngle label is updated continuously

            correctHingeAngle = (CorrectHingeAngle)target;
            EditorGUILayout.LabelField("angle:", correctHingeAngle.currentAngle.ToString());
            EditorGUILayout.LabelField("angular velocity:", correctHingeAngle.currentVelocity.ToString());
        }
    }
}
