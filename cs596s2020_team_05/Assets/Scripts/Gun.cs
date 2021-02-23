using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : MonoBehaviour
{
    public bool gunActive = false;
    //
    public float fireTimer;
    public float fireRate;
    public int indexFireHold = -1;
    public int amountFireGun = 1; // >= 1
    public float rotationWeaponSpeed = 10f;
    //
    public Transform[] fireHolds;
    //
    public GameObject weaponObj;
    public GameObject bullet;
    public GameObject bulletMuzzle;
    public GameObject bulletSmoke;
    //
    public bool playerGunAttack = false;
    //
    public bool isReload = false;   // true- is reloading ; false - by cooldown  
    public float reloadTime = 1.5f;
    public float reloadTimer = 0;
    public int maxAmmo = 30;
    public int currentAmmo = 0;
    //
    public AudioClip audioClip_Shoot;
    public AudioClip audioClip_Reload;
    //---------------   
    private Transform[] gunActiveSlotArr;
    public Transform[] GetGunActiveSlotArr
    { get { return gunActiveSlotArr; } set { gunActiveSlotArr = value; } }
    //----------   

    //------------
    public void ActivateWeapon()
    {
        weaponObj.SetActive(true);
        gunActive = true;
        playerGunAttack = true;
        isReload = true;

    }

    public void SetAmoutFireGun(int amout)
    {
        int currentActiveGun = checkValueElement(fireHolds);
        if (amout <= currentActiveGun)
        {
            amountFireGun = amout;
        }
    }

    public void SetActiveGunSlot(int[] gunIndexArr)
    {
        bool activeFlag = true;
        int gunIndex = gunIndexArr.Length;
        // Debug.Log("gunindex : " + gunIndex);
        int currentActiveGun = checkValueElement(fireHolds);
        // Debug.Log("currentActiveGun : " + currentActiveGun);
        if ((gunIndex - 1) > fireHolds.Length)  // subtract negative number 1
        {
            Debug.Log("Error SetActiveGunSlot "); return;
        }

        for (int i = 0; i < gunIndex; i++)
        {
            int gunNum = gunIndexArr[i];
            if (gunNum == -1)
            {
                activeFlag = false;
                continue;
            }

            if (gunNum < fireHolds.Length)
            {
                fireHolds[gunNum].gameObject.SetActive(activeFlag);
                if (!activeFlag)
                {
                    foreach (Transform tr in fireHolds[gunNum].gameObject.transform)
                    {
                        GameObject.Destroy(tr.gameObject);
                    }
                }

            }

        }
    }

    public int checkValueElement(Transform[] fireHold)
    {
        gunActiveSlotArr = new Transform[fireHold.Length];

        int count = 0;

        for (int i = 0; i < fireHold.Length; i++)
        {
            if (fireHold[i] != null && fireHold[i].gameObject.activeSelf)
            {
                gunActiveSlotArr[count] = fireHold[i];
                count++;


            }

        }
        return count;
    }

    public int GetPositionFireHold(int _positionFire, int lengthFireHold)
    {
        _positionFire++; // start at -1
        if (_positionFire >= lengthFireHold) //>=
        {
            _positionFire = 0;
        }
        //Debug.Log("position fire: " + _positionFire);
        return _positionFire;
    }

    public void PlaySoundType1(AudioSource audioSource, AudioClip audioClip)
    {
        if(audioClip != null)
            audioSource.PlayOneShot(audioClip);
    }

    public void PlaySoundType2(AudioSource audioSource, AudioClip audioClip)
    {
        if (audioClip != null &&!audioSource.isPlaying )
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
