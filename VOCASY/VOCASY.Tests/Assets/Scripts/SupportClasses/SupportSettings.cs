using VOCASY;
public class SupportSettings : VoiceChatSettings
{
    public ushort MinF = 12000;
    public ushort MaxF = 48000;
    public byte MinC = 1;
    public byte MaxC = 2;
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