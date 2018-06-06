using UnityEngine;
using VOCASY;
using VOCASY.Common;
using System.Collections.Generic;
public class SelfStarter : MonoBehaviour
{
    public VoiceDataWorkflow Workflow;
    public GameObject Prefab;

    void Start()
    {
        Transport selfT = Workflow.Transport as Transport;
        if (selfT != null)
        {
            selfT.SendMsgTo = null;
            selfT.SendToAllAction = (byte[] d, int s, int l, List<ulong> rs) => Workflow.ProcessReceivedPacket(d, s, l, rs[0]);
        }

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

        Destroy(this);
    }
}