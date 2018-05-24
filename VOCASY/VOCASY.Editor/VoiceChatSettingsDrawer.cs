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
        /// Method that modifies VoiceChatSettings inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            VoiceChatSettings settings = target as VoiceChatSettings;
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Current Settings"))
                {
                    settings.SaveCurrentSettings();
                }

                else if (GUILayout.Button("Restore to Saved Settings"))
                {
                    settings.RestoreToSavedSettings();
                }
            }
        }
    }
}