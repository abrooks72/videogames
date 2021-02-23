using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyAttack : MonoBehaviourPunCallbacks, IPunObservable
{
    private Radar _enemyRadar;
    private EnemyMove _enemyMove;
    //
    //private bool flagTrigger = false; // true to setTarget from collider sphere
    private GameObject obj = null;   // this avarible will recieve a OBJ from  CircleDectPlayer  
    private Transform target;
    private bool shootFlag = false;

    [SerializeField] private Gun[] GunArray;
    AudioSource audioSource;

    public GameObject RotationOBJ = null;
    public GameObject RoatationWeapon = null;
    [SerializeField] private bool RotateObjFlag = false;
    [SerializeField] private bool RotateWeaponFlag = false;
    //
    public float RotateObjSpeed = 10f;
    public float RotateWeaponSpeed = 10f;
    public float waitAttackTime = 1f;
    public float waitAttackTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRadar = GetComponent<Radar>();
        if (!_enemyRadar)
        {
            _enemyRadar = transform.GetChild(2).GetChild(0).GetComponent<Radar>();
        }
        //
        _enemyMove = GetComponent<EnemyMove>();

        //
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].gunActive)
            {
                GunArray[i].currentAmmo = GunArray[i].maxAmmo;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //reload
        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].gunActive)
                ReloadAmmo(i);
        }
        //
        //    checkShootFlag();
        AutoFighter();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (RoatationWeapon != null)
                stream.SendNext(RoatationWeapon.transform.rotation);
            if (RotationOBJ != null)
                stream.SendNext(RotationOBJ.transform.rotation);
        }
        else if (stream.IsReading)
        {
            if (RoatationWeapon != null)
                RoatationWeapon.transform.rotation = (Quaternion)stream.ReceiveNext();
            if (RotationOBJ != null)
                RotationOBJ.transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }
    //-------------------------------
    private void AutoFighter()
    {
        obj = _enemyRadar.GetBestTarget;
        if (obj != null) // if target puts in is available 
        {
            //
            faceDirection();
            // obj move        
           
            for (int i = 0; i < GunArray.Length; i++)
            {

                if (GunArray[i].gunActive && PhotonNetwork.IsConnected)
                {
                    if (GetComponent<PhotonView>().IsMine)
                        GetComponent<PhotonView>().RPC("Shoot", RpcTarget.AllBuffered, i);
                    
                    //Shoot(i); // 0 is the first weapon in the list GunArray
                }
            }

        }

    }

    public void checkShootFlag()
    {
        if (obj != null)
            shootFlag = true;
        else
            shootFlag = false;

    }

    void faceDirection()
    {
        target = obj.transform;
        if (RotateObjFlag)
            FaceTarget(RotationOBJ);
        if (RotateWeaponFlag)
            RotateWeapon(RoatationWeapon);

    }

    [PunRPC]
    private void Shoot(int num)  //num = gunindex
    {
        //
        waitAttackTimer += Time.deltaTime;
        //
        if (GunArray[num].fireTimer < GunArray[num].fireRate)
        {
            GunArray[num].fireTimer += Time.deltaTime;
        }
        //

        // target = obj.transform; // set input target to current object "target"
        //edited
        // if (RotateObjFlag)
        //     FaceTarget(RotationOBJ);
        // if (RotateWeaponFlag)
        //     RotateWeapon(RoatationWeapon);

        if (waitAttackTimer >= waitAttackTime)
        {
            waitAttackTimer = 0;
            if (GunArray[num].fireTimer > GunArray[num].fireRate && !GunArray[0].playerGunAttack) //Input.GetButton("Fire1") && 
            {

                if (GunArray[num].currentAmmo > 0)
                {
                    GunArray[num].PlaySoundType1(audioSource, GunArray[num].audioClip_Shoot);

                    GunArray[num].fireTimer = 0;
                    GunArray[num].playerGunAttack = true;
                    int valueFireHold = GunArray[num].checkValueElement(GunArray[num].fireHolds); // active gun slot

                    for (int i = 0; i < GunArray[num].amountFireGun; i++)
                    {
                        GunArray[num].currentAmmo--;

                        GunArray[num].indexFireHold = GunArray[num].GetPositionFireHold(GunArray[num].indexFireHold, valueFireHold);
                        // print("intdexFireHold " + GunArray[num].indexFireHold);
                        // print("valueFireHold " + valueFireHold);

                        Transform fire_Hold_T = GunArray[num].GetGunActiveSlotArr[GunArray[num].indexFireHold];
                        Vector3 fire_Hold_P = fire_Hold_T.position;
                        Quaternion fire_Hold_R = fire_Hold_T.rotation;

                        GameObject bullet = Instantiate(GunArray[num].bullet, fire_Hold_P, fire_Hold_R);
                        bullet.GetComponent<ImpactCollisonBullet>().photonView = GetComponent<PhotonView>();
                        Instantiate(GunArray[num].bulletMuzzle, fire_Hold_P, fire_Hold_R);
                        Instantiate(GunArray[num].bulletSmoke, fire_Hold_P, fire_Hold_R, fire_Hold_T);
                        //

                        //
                        GunArray[num].playerGunAttack = false;
                        if (GunArray[num].currentAmmo <= 0)
                        {
                            GunArray[num].isReload = true;
                            break;
                        }
                    }
                }




                if (GunArray[num].currentAmmo <= 0)
                {
                    GunArray[num].isReload = true;
                }
            }
        }
        //






    }
    //--------------------------------
    private void ReloadAmmo(int num) // num = gunIndex
    {
        if (GunArray[num].isReload)
        {
            GunArray[num].playerGunAttack = true;
            GunArray[num].PlaySoundType2(audioSource, GunArray[num].audioClip_Reload);
            GunArray[num].reloadTimer += Time.deltaTime;
            if (GunArray[num].reloadTimer >= GunArray[num].reloadTime)
            {
                GunArray[num].reloadTimer = 0;
                Reload(num);
            }

        }
    }
    private void Reload(int num) // num = gunIndex
    {
        if (GunArray[num].isReload)
        {
            GunArray[num].isReload = false;
            GunArray[num].currentAmmo = GunArray[num].maxAmmo;
            GunArray[num].playerGunAttack = false;
        }
    }


    //--------------------------------
    void RotateWeapon(GameObject objWeapon)
    {
        if (objWeapon == null)
        {
            print("target is null");
            return;

        }

        Vector3 direction = (target.position - objWeapon.transform.position).normalized; //ChangePositionTarget() - transform.position   

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        objWeapon.transform.rotation = Quaternion.Lerp(objWeapon.transform.rotation, lookRotation, Time.deltaTime * RotateWeaponSpeed);
    }

    void FaceTarget(GameObject objPart) // ~ turn when move 
    {
        if (objPart == null) return;
        Vector3 direction = (target.position - objPart.transform.position).normalized; //ChangePositionTarget() - transform.position       
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        objPart.transform.rotation = Quaternion.Lerp(objPart.transform.rotation, lookRotation, Time.deltaTime * RotateObjSpeed);


    }
    //-------------------------------------------------------
    // public void SetActivateTrigger(bool flag) // used in CircleDetectPlayer - 1
    // { flagTrigger = flag; }

    // public void SetTarget(GameObject _obj) // used in CircleDectPlayer - 2
    // {
    //     if (flagTrigger)
    //     { obj = _obj.gameObject; }
    // }

    // public void ResetTarget()
    // {
    //     obj = null;
    // }
    //---------------------------------------------------
    void OnDrawGizmos()
    {


        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(RoatationWeapon.transform.position, 0.1f);
        // Gizmos.color = Color.green;
        // Gizmos.DrawSphere(target.transform.position, 1f);
    }
}
