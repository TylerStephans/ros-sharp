// Modified JointStatePatcherEditor to work with JointCommandPatcher
// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;
using UnityEditor;

namespace RosSharp.RosBridgeClient
{
    [CustomEditor(typeof(JointCommandPatcher))]
    public class JointCommandPatcherEditor : Editor
    {
        private JointCommandPatcher jointCommandPatcher;
        private static GUIStyle buttonStyle;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (buttonStyle == null)
                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight) { fixedWidth = 75 };

            jointCommandPatcher = (JointCommandPatcher)target;

            GUILayout.Label("All Urdf Joints", EditorStyles.boldLabel);
            // I don't have a need to publish commands so I removed this
            /*
            DisplaySettingsToggle(new GUIContent("Publish Joint Command", "Adds/removes a Joint Command Reader on each joint."),
                jointCommandPatcher.SetPublishJointCommands);
                */
            DisplaySettingsToggle(new GUIContent("Subscribe Joint Command", "Adds/removes a Joint Command Writer on each joint."),
                jointCommandPatcher.SetSubscribeJointCommands);
        }

        private delegate void SettingsHandler(bool enable);

        private static void DisplaySettingsToggle(GUIContent label, SettingsHandler handler)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            if (GUILayout.Button("Enable", buttonStyle))
                handler(true);
            if (GUILayout.Button("Disable", buttonStyle))
                handler(false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
