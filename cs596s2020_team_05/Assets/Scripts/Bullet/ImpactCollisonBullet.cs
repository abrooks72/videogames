using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ImpactCollisonBullet : MonoBehaviour
{
    Bullet _projectile;  
    public PhotonView photonView;
    // Start is called before the first frame update
    private void Start()       
    {      
        _projectile = GetComponent<Bullet>();
    }

    void OnCollisionEnter(Collision other)
    {   
        if(_projectile == null) return;     
        if (_projectile.GetHitEffectArr != null && other.gameObject.tag != _projectile.GetCharacter1 ) 
        {           
            foreach (GameObject hit in _projectile.GetHitEffectArr)
            {
                if (hit != null)
                {                   
                   GameObject obj =  Instantiate(hit, transform.position, transform.rotation);   
                   Destroy(gameObject,_projectile.GetLifeAfterImpact);
                }
            }
        }
        //------------
        if (other.gameObject.tag == _projectile.GetCharacter2) // the one need hurt 
        {
            _projectile.GetCount++;
            if (_projectile.GetCount < 2)
            {
                //----------               
                if(other.gameObject.GetComponent<HealthBase>())
                {
                    if(photonView.IsMine)
                        other.gameObject.GetComponent<PhotonView>().RPC("Hit",RpcTarget.All,_projectile.GetDamage);
                    //other.gameObject.GetComponent<HealthBase>().Hit(1f);
                }
                    //other.gameObject.GetComponent<HealthBase>().Hit(1f);       // damage       
                //
                if (other.gameObject.tag == "Enemy")
                {
                    // do something
                }
            }
        }
        //          
    }

   
}
