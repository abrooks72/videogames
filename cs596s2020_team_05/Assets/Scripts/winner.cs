using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class winner : MonoBehaviour
{
    public GameObject winPannel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            GetComponent<PhotonView>().RPC("win",RpcTarget.AllBuffered,null);

        }
    }
    
    [PunRPC]
    public void win()
    {
        winPannel.SetActive(true);
        Invoke("leave",2f);
    }

    void leave()
    {
        GameNetManager.instance.LeaveRoom();
    }
}
