using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
   
    private PlayerMove _playerMove = null;
    private Radar _playerRadar = null;
    private PlayerAttack _playerAttack = null;
    // Start is called before the first frame update
    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();    //  status player movement
        _playerRadar = GetComponent<Radar>();   // obj , distance of target 
        _playerAttack = GetComponent<PlayerAttack>();
    }
    //
    void FixedUpdate()
    {
        // if(photonView.IsMine)
        // _playerMove.GetComponent<PhotonView>().RPC("InputPlayerMove", RpcTarget.AllBuffered,null);  // move player     
        _playerMove.InputPlayerMove();    
    }
    //
    void Update()
    {        
        DetectedEnemy();
    }
    //
    private void DetectedEnemy()
    {
        // if (!_playerAttack.GetPlayerAttack)
        // {
        GameObject target = _playerRadar.GetBestTarget;
        if ((target != null && target.activeSelf && !target.GetComponent<HealthBase>().GetDeath))
        {
            for (int i = 0; i < _playerAttack.GetGunArray.Length; i++)
            {
                if (_playerAttack.GetGunArray[i].gunActive)
                {
                    //print("PlayerCOntrol: "+ i);
                    _playerAttack.lookTarget(_playerMove.GetWeaponPlayerArr[0], _playerMove.GetWeaponPlayerArr[0], target, i);


                }
                    
            }

            StartCoroutine(_playerAttack.CoolDownWaitFaceTarget(0.1f));    //0.1f shoot
        }
        // //test shoot
        // if(Input.GetKey(KeyCode.Space))
        // StartCoroutine(_playerAttack.CoolDownWaitFaceTarget(0.2f));


        //  }

    }
    //


}// End PlayerController
