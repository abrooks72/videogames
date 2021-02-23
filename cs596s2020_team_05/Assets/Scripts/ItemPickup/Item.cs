using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    public string itemName = "";
    int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            count++;
            if (count < 2)
            {
                print("request setweapon " + count +" time.");               
                other.gameObject.GetComponent<PlayerAttack>().SetWeapon(itemName);
                //Destroy(GetComponentInParent<Transform>().gameObject);
                GetComponent<PhotonView>().RPC("DetroyItem",RpcTarget.AllBuffered,null);
            }
        }
    }

    [PunRPC]
    private void DetroyItem()
    {
        Destroy(GetComponentInParent<Transform>().gameObject);
    }




}
