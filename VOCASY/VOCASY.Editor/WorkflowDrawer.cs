using UnityEngine;
using UnityEditor;
using VOCASY.Common;
namespace VOCASY.Editor
{
    /// <summary>
    /// Class that modifies Workflow inspector view
    /// </summary>
    [CustomEditor(typeof(Workflow))]
    public class WorkflowsDrawer : UnityEditor.Editor
    {
        Workflow workflow;
        private void OnEnable()
        {
            workflow = target as Workflow;
        }
        /// <summary>
        /// Method that modifies Workflow inspector view
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Players Mute Status", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Save", "Saved mute statuses will be overwritten by current statuses", "Save", "Cancel"))
                        workflow.SaveCurrentMuteStatuses();
                }

                else if (GUILayout.Button("Load Saved Players Mute Status", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Load", "By loading saved values the unsaved statuses may be lost", "Load", "Cancel"))
                        workflow.LoadSavedMuteStatuses();
                }
            }
            if (GUILayout.Button("Clear Saved Players Mute Status", EditorStyles.miniButton))
                if (EditorUtility.DisplayDialog("Clear", "Are you sure to delete the file containing the clients mute statuses?", "Clear", "Cancel"))
                    workflow.LoadSavedMuteStatuses();
        }
    }
}