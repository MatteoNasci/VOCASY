using UnityEngine;
using UnityEditor;
using VOCASY.Common;
namespace VOCASY.Editor
{
    /// <summary>
    /// Class that modifies VoiceChatSettings inspector view
    /// </summary>
    [CustomEditor(typeof(VoiceChatSettings))]
    public class VoiceChatSettingsDrawer : UnityEditor.Editor
    {
        /// <summary>
        /// Method that modifies SOEvent inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //GUI.enabled = Application.isPlaying;

            VoiceChatSettings e = target as VoiceChatSettings;
            if (GUILayout.Button("Save Current Settings"))
                e.SaveCurrentSettings();
            else if (GUILayout.Button("Restore to Saved Settings"))
                e.RestoreToSavedSettings();
        }
    }
}