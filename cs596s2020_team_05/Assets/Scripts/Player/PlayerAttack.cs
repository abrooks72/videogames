using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class PlayerAttack : MonoBehaviourPunCallbacks//,IPunObservable
{
    private Radar _playerRadar = null;
    [SerializeField] private Gun[] GunArray;
    private bool playerAttack = false;       // true- player attacks;   false - by cooldown  

    public bool GetPlayerAttack
    { get { return playerAttack; } set { playerAttack = value; } }
    public Gun[] GetGunArray
    { get { return GunArray; } set { GunArray = value; } }
    //
    AudioSource audioSource;
    // 0: shoot; 1:reloadAmmo; 
    //
    public event Action<float, float, int> OnAmmoAmountChanged = delegate { };

    public void ModifyAmmoDisplayNumber(float ammo_current, float ammo_total, int gunIndex)
    {
        OnAmmoAmountChanged(ammo_current, ammo_total, gunIndex);
    }
    //-------------------------------------------------------------------------------------    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _playerRadar = GetComponent<Radar>();
        //        

        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].gunActive)
            {
                GunArray[i].currentAmmo = GunArray[i].maxAmmo;
                ModifyAmmoDisplayNumber(GunArray[i].currentAmmo, GunArray[i].maxAmmo, i); //i = gunIndex
            }
        }



    }
    //--------------------------


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            int[] gunActiveSlot = { 1, 2 };
            int[] gunDeActiveSlot = { 0 };
            int[] gunSlot = { 1, 2, -1, 0 };
            GunArray[0].SetActiveGunSlot(gunSlot);
        }
        //
        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].gunActive)
                ReloadAmmo(i);
        }



    }


    //------------
    public void SetWeapon(string weaponName)
    {
        print("set-up weapon name: " + weaponName);
        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].weaponObj.name.Equals(weaponName))
            {
                print("Found Weapon:  " + weaponName + " index: " + i);
                //GunArray[i].ActivateWeapon();
                if (GetComponent<PhotonView>().IsMine)
                    GetComponent<PhotonView>().RPC("activeWp", RpcTarget.AllBuffered, i);
            }
        }

    }
    [PunRPC]
    private void activeWp(int i)
    {
        GunArray[i].ActivateWeapon();
    }
    //***********************************************************************
    private void ReloadAmmo(int num) // num = gunIndex
    {
        if (GunArray[num].isReload)
        {
            if (photonView.IsMine)
            {
                GunArray[num].PlaySoundType2(audioSource, GunArray[num].audioClip_Reload);
            }
            GunArray[num].playerGunAttack = true;
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
            ModifyAmmoDisplayNumber(GunArray[num].currentAmmo, GunArray[num].currentAmmo, num);
            GunArray[num].playerGunAttack = false;

        }
    }


    //-------------------------
    public IEnumerator CoolDownWaitFaceTarget(float time)  // used from PlayerControl class
    {
        yield return new WaitForSeconds(time);
        AutoFighter();
    }

    private void AutoFighter()
    {
        
        for (int i = 0; i < GunArray.Length; i++)
        {
            if (GunArray[i].gunActive)
            {
                if (GetComponent<PhotonView>().IsMine)
                    GetComponent<PhotonView>().RPC("Shoot", RpcTarget.AllBuffered, i);
                //Shoot(i); // 0 is the first weapon in the list GunArray

            }

        }

    }

    [PunRPC]
    private void Shoot(int num)  //num = gunindex
    {
        if (GunArray[num].fireTimer < GunArray[num].fireRate)
        {
            GunArray[num].fireTimer += Time.deltaTime;
        }

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
                    ModifyAmmoDisplayNumber(GunArray[num].currentAmmo, GunArray[num].maxAmmo, num);

                    GunArray[num].indexFireHold = GunArray[num].GetPositionFireHold(GunArray[num].indexFireHold, valueFireHold);
                    // print("intdexFireHold " + GunArray[num].indexFireHold);
                    // print("valueFireHold " + valueFireHold);

                    Transform fire_Hold_T = GunArray[num].GetGunActiveSlotArr[GunArray[num].indexFireHold];
                    Vector3 fire_Hold_P = fire_Hold_T.position;
                    Quaternion fire_Hold_R = fire_Hold_T.rotation;
                    //
                    GameObject bullet = Instantiate(GunArray[num].bullet, fire_Hold_P, fire_Hold_R);
                    bullet.GetComponent<ImpactCollisonBullet>().photonView = GetComponent<PhotonView>();
                    Instantiate(GunArray[num].bulletMuzzle, fire_Hold_P, fire_Hold_R);
                    Instantiate(GunArray[num].bulletSmoke, fire_Hold_P, fire_Hold_R, fire_Hold_T);
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
    //------------------



    //--------------------------------
    public void lookTarget(GameObject modelPlayer, GameObject weaponPlayer, GameObject target, int gunIndex)  // used in Fighter > Update() > DetectedEnemy()
    {
        // wait for 0.1f because lookTarget runs on Update, but it have to wait for enable sphere collider .
        // enable sphere collider run on OnCollider Enter() is slow than the one runs on Update().
        StartCoroutine(CoolDownWaitForScan(0.1f, modelPlayer, weaponPlayer, target, gunIndex));
    }

    private IEnumerator CoolDownWaitForScan(float time, GameObject modelPlayer, GameObject weaponPlayer, GameObject target, int gunIndex)
    {
        yield return new WaitForSeconds(time); // after wait for OnCollider Enter() detects obj
        if (_playerRadar.GetArrayEnemyStruct != null)
        {
            AimTarget(1, modelPlayer, weaponPlayer, target, gunIndex); // look at the target on the array that is collected by sphere collider. 
        }
    }

    void AimTarget(int amountTarget, GameObject modelPlayer, GameObject weaponPlayer, GameObject target, int gunIndex) // 1 or N obj 
    {
        for (int i = 0; i < amountTarget; i++)
        {

            if (GunArray[gunIndex].gunActive)
            {
                if (target != null) //!GunArray[gunIndex].playerGunAttack
                {
                    // if (target.GetComponent<EnemyChase>() != null)
                    // {
                    FaceTarget(target, modelPlayer, weaponPlayer, GunArray[gunIndex].rotationWeaponSpeed);  //GunArray[gunIndex].rotationWeaponSpeed
                    //print("target health is on ");
                    //  }
                }

            }




        }

    }

    void FaceTarget(GameObject target, GameObject currentObject, GameObject weaponPlayer, float lookAtSpeed)
    {
        if (target != null)
        {
            float height = 0;//(target.GetComponent<NavMeshAgent>().height / 2);
            Vector3 obj = new Vector3
                (target.transform.position.x,
                target.transform.position.y + height,
                target.transform.position.z);  //
            Vector3 lookDirection = (obj - currentObject.transform.position).normalized;

            //float yRotation = lookDirection.y;
            // if (yRotation > 0.5f) yRotation = 0.5f;
            // else if (yRotation < -0.5f) yRotation = -0.5f;

            Quaternion lookRotation_model = Quaternion.LookRotation(new Vector3(
                    lookDirection.x,
                    lookDirection.y,
                    lookDirection.z));  //y//x//z bi dao nguoc xy           

            currentObject.transform.rotation = Quaternion.Lerp(currentObject.transform.rotation, lookRotation_model, Time.deltaTime * lookAtSpeed);
            //--
            // Quaternion lookRotation_hands = Quaternion.LookRotation(new Vector3(
            //         lookDirection.x,
            //         lookDirection.y,
            //         lookDirection.z));  //y//x//z bi dao nguoc xy
            // weaponPlayer.transform.rotation = Quaternion.Lerp(weaponPlayer.transform.rotation, lookRotation_hands, Time.deltaTime * lookAtSpeed);

            //           print("2 "+lookDirection);
        }
        else
        {
            print("facing null target ");
        }
    }


}
