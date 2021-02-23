using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject Camera;
    //[SerializeField] GameObject UI;
    //
    [SerializeField] GameObject AmmoGui;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] GameObject bodyObj;
    [SerializeField] Material[] materials;

    //
    public float activeTimer = 0;
    public float activeTime = 1f;
    public bool activeFlag = false;



    // Start is called before the first frame update
    void Start()
    {
        // disable all "camera,movement" of player is not the current player
        if (GetComponent<PhotonView>().IsMine)
        {

            GetComponent<PlayerController>().enabled = true;           
            GetComponent<PlayerMove>().enabled = true;
            GetComponent<Radar>().enabled = true;
            GetComponent<PlayerAttack>().enabled = true;
            GetComponent<HealthBase>().enabled = false;
            bodyObj.SetActive(false);
            AmmoGui.SetActive(false);
            //Camera.GetComponent<Camera>().enabled = true;
            Camera.GetComponent<AudioListener>().enabled = true;
            
            //-----------
            SetPlayerColor();

        }
        else
        {
            GetComponent<PlayerController>().enabled = false;
          //  GetComponent<HealthBase>().enabled = false;
            GetComponent<PlayerMove>().enabled = false;
            GetComponent<Radar>().enabled = false;
           // GetComponent<PlayerAttack>().enabled = false;
            Camera.GetComponent<Camera>().enabled = false;
            Camera.GetComponent<AudioListener>().enabled = false;
            AmmoGui.SetActive(false);

        }

        SetPlayerName();
    }

    private void Update()
    {
        if (!activeFlag)
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                activeTimer += Time.deltaTime;
                if (activeTimer >= activeTime)
                {
                   // Debug.LogError("pass");
                    activeTimer = 0;
                    activeFlag = true;
                    //
                    AmmoGui.SetActive(true);
                    GetComponent<HealthBase>().enabled = true;
                    Camera.GetComponent<Camera>().enabled = true;
                    bodyObj.SetActive(true);
                    GetComponent<PlayerController>().enabled = true;
                    Camera.GetComponent<Camera>().enabled = true;
                    Camera.GetComponent<AudioListener>().enabled = true;
                    AmmoGui.SetActive(true);
                }

            }

        }

    }

    void SetPlayerName()
    {
        if (playerNameText != null)
        {
            playerNameText.text = photonView.Owner.NickName;
        }
    }

    void SetPlayerColor()
    {
        int count = PhotonNetwork.CurrentRoom.PlayerCount;
        while (count > materials.Length) // limit setup 4 different color player 
        {
            count -= 4;
        }

        gameObject.GetComponent<PhotonView>().RPC("SetColor", RpcTarget.AllBuffered, count - 1);
    }

    [PunRPC]
    void SetColor(int count)
    {
        bodyObj.GetComponent<Renderer>().material = materials[count];
    }

}
