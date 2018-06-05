using UnityEngine;
using System.IO;
namespace VOCASY.Common
{
    /// <summary>
    /// Class that manages and holds voice chat settings
    /// </summary>
    [CreateAssetMenu(menuName = "VOCASY/Settings")]
    public class Settings : VoiceChatSettings
    {
        /// <summary>
        /// Minimum frequency possible
        /// </summary>
        public const ushort MinFreq = (ushort)FrequencyType.LowerThanAverageQuality;
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public const ushort MaxFreq = (ushort)FrequencyType.BestQuality;
        /// <summary>
        /// Minimum channels possible
        /// </summary>
        public const byte MinChan = 1;
        /// <summary>
        /// Maximum channels possible
        /// </summary>
        public const byte MaxChan = 2;
        /// <summary>
        /// Minimum frequency possible
        /// </summary>
        public override ushort MinFrequency { get { return MinFreq; } }
        /// <summary>
        /// Maximum frequency possible
        /// </summary>
        public override ushort MaxFrequency { get { return MaxFreq; } }
        /// <summary>
        /// Minimum channels possible
        /// </summary>
        public override byte MinChannels { get { return MinChan; } }
        /// <summary>
        /// Maximum channels possible
        /// </summary>
        public override byte MaxChannels { get { return MaxChan; } }

        /// <summary>
        /// Restore the settings to the saved file values. If file is not found it is created with current settings values
        /// </summary>
        public override void RestoreToSavedSettings()
        {
            if (File.Exists(SavedCustomValuesPath))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(SavedCustomValuesPath), this);
            else
                SaveCurrentSettings();
        }
        /// <summary>
        /// Saves current settings to file. If it is not performed changes to the settings will not be recorded
        /// </summary>
        public override void SaveCurrentSettings()
        {
            if (!Directory.Exists(SavedCustomValuesDirectoryPath))
                Directory.CreateDirectory(SavedCustomValuesDirectoryPath);

            File.WriteAllText(SavedCustomValuesPath, JsonUtility.ToJson(this));
        }
    }
}