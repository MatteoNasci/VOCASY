using VOCASY;
public class SupportSettings : VoiceChatSettings
{
    public ushort MinF;
    public ushort MaxF;
    public byte MinC;
    public byte MaxC;
    public bool RestoreSettings = false;
    public bool SaveSettings = false;
    public override ushort MinFrequency { get { return MinF; } }

    public override ushort MaxFrequency { get { return MaxF; } }

    public override byte MinChannels { get { return MinC; } }

    public override byte MaxChannels { get { return MaxC; } }

    public override void RestoreToSavedSettings()
    {
        RestoreSettings = true;
    }
    public override void SaveCurrentSettings()
    {
        SaveSettings = true;
    }
}