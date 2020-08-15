// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;
using UnityEditor;

namespace RosSharp.RosBridgeClient
{
    [CustomEditor(typeof(ClockPublisher))]
    public class ClockPublisherEditor : UnityEditor.Editor
    {
        private ClockPublisher clockPublisher;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUtility.SetDirty(target); // Necessary so that publishedTime label is updated continuously

            clockPublisher = (ClockPublisher)target;
            EditorGUILayout.LabelField("Published Time:", clockPublisher.publishedTime);
        }
    }
}
