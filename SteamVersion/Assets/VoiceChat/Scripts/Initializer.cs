using UnityEngine;
using VOCASY;
using VOCASY.Common;
public class Initializer : MonoBehaviour
{
    public VoiceHandler Prefab;
    public VoiceDataWorkflow Workflow;
    public ulong selfId = 1;
    void Start()
    {

        SelfDataTransport selfT = Workflow.Transport as SelfDataTransport;
        if (selfT != null)
        {
            VoiceHandler obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<VoiceHandler>();
            obj.Identity.IsLocalPlayer = true;
            obj.Identity.NetworkId = selfId;
            obj.Identity.IsInitialized = true;

            obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<VoiceHandler>();
            obj.Identity.IsLocalPlayer = false;
            obj.Identity.NetworkId = selfT.ReceiverId;
            obj.Identity.IsInitialized = true;
        }

        Destroy(this);
    }
}