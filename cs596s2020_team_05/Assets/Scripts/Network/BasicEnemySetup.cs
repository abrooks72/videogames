using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicEnemySetup : MonoBehaviourPunCallbacks
{
    Radar _enemyRadar;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
        {            
            GetComponent<HealthBase>().enabled = true;

            _enemyRadar = GetComponent<Radar>();
            if (!_enemyRadar)
            {
                _enemyRadar = transform.GetChild(2).GetChild(0).GetComponent<Radar>();                
            }
            _enemyRadar.enabled = true;

            GetComponent<EnemyAttack>().enabled = true;  

        }
        else
        {

            // GetComponent<HealthBase>().enabled = false;
            _enemyRadar = GetComponent<Radar>();
            if (!_enemyRadar)
            {
                _enemyRadar = transform.GetChild(2).GetChild(0).GetComponent<Radar>();                
            }
            _enemyRadar.enabled = false;
           // GetComponent<EnemyAttack>().enabled = false;


        }
        
    }

    
}
