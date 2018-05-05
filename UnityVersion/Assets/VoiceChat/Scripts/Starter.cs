using UnityEngine;
using VOCASY;
using VOCASY.Common;
[RequireComponent(typeof(IAudioTransportLayer))]
public class Starter : MonoBehaviour
{
    public VoiceChatSettings Settings;
    public GameObject Prefab;
    private uint otherId = 5;
    private uint selfId = 1;
    void Start()
    {
        IAudioTransportLayer transport = GetComponent<IAudioTransportLayer>();
        Settings.AudioQuality = FrequencyType.BestQuality;
        VoiceDataWorkflow.Init(new ConcentusDataManipulator(VoiceChatSettings.MaxFrequency, 1), transport, Settings, VoiceChatSettings.MaxFrequency, 2);

        SelfDataTransport selfT = transport as SelfDataTransport;
        if (selfT != null)
        {
            selfT.ReceiverId = otherId;

            INetworkIdentity obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<INetworkIdentity>();
            obj.IsLocalPlayer = true;
            obj.NetworkId = selfId;

            obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<INetworkIdentity>();
            obj.IsLocalPlayer = false;
            obj.NetworkId = otherId;
        }

        Destroy(this);
    }
}