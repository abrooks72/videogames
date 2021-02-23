using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TriggerActivateEnemy : MonoBehaviour
{
   
    public bool flagTrigger;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(!flagTrigger)
            {
                flagTrigger = true;
                GetComponent<PhotonView>().RPC("RPCSetActiveItem", RpcTarget.AllBuffered, null);

            }
           
        }

    }

    [PunRPC]
    private void RPCSetActiveItem()
    {
        Invoke("SetActiveItem",0.2f);

    }

    private void SetActiveItem()
    {
        gameObject.SetActive(false);
    }


    
}
