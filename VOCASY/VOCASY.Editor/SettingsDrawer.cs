using UnityEngine;
using UnityEditor;
using VOCASY.Common;
namespace VOCASY.Editor
{
    /// <summary>
    /// Class that modifies VoiceChatSettings inspector view
    /// </summary>
    [CustomEditor(typeof(Settings))]
    public class SettingsDrawer : UnityEditor.Editor
    {
        Settings settings;
        private void OnEnable()
        {
            settings = target as Settings;
        }
        /// <summary>
        /// Method that modifies VoiceChatSettings inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Current Settings", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Save", "Saved settings will be overwritten by current settings", "Save", "Cancel"))
                        settings.SaveCurrentSettings();
                }

                else if (GUILayout.Button("Restore to Saved Settings", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Restore", "By restoring saved values the unsaved settings will be lost", "Restore", "Cancel"))
                        settings.RestoreToSavedSettings();
                }
            }
        }
    }
}