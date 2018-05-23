using UnityEngine;
using VOCASY;
using VOCASY.Common;
public class Initializer : MonoBehaviour
{
    public VoiceHandler Prefab;
    public VoiceDataWorkflow Workflow;
    private ulong otherId = 5;
    private ulong selfId = 1;
    void Start()
    {

        SelfDataTransport selfT = Workflow.Transport as SelfDataTransport;
        if (selfT != null)
        {
            selfT.ReceiverId = otherId;

            VoiceHandler obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<VoiceHandler>();
            obj.Identity.IsLocalPlayer = true;
            obj.Identity.NetworkId = selfId;
            obj.Identity.IsInitialized = true;

            obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<VoiceHandler>();
            obj.Identity.IsLocalPlayer = false;
            obj.Identity.NetworkId = otherId;
            obj.Identity.IsInitialized = true;
        }

        Destroy(this);
    }
}