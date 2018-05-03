using UnityEngine;
using VOCASY;
[RequireComponent(typeof(IAudioTransportLayer))]
public class Initializer : MonoBehaviour
{
    public VoiceChatSettings Settings;
    void Update()
    {
        if (SteamManager.Initialized)
        {
            VoiceDataWorkflow.Init(new SteamVoiceDataManipulator(), GetComponent<IAudioTransportLayer>(), Settings);
            Destroy(this);
        }
    }
}