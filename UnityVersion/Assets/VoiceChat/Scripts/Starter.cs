using UnityEngine;
using VOCASY;
using VOCASY.Common;
public class Starter : MonoBehaviour
{
    public VoiceDataWorkflow Workflow;
    public GameObject Prefab;

    void Start()
    {
        SelfDataTransport selfT = Workflow.Transport as SelfDataTransport;
        if (selfT != null)
        {
            Handler obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<Handler>();
            if (obj)
            {
                obj.Identity = new NetworkIdentity();
                obj.Identity.IsLocalPlayer = true;
                obj.Identity.NetworkId = 1;
                obj.Identity.IsInitialized = true;
            }

            obj = GameObject.Instantiate<GameObject>(Prefab.gameObject).GetComponent<Handler>();
            if (obj)
            {
                obj.Identity = new NetworkIdentity();
                obj.Identity.IsLocalPlayer = false;
                obj.Identity.NetworkId = 2;
                obj.Identity.IsInitialized = true;
            }
        }

        Destroy(this);
    }
}