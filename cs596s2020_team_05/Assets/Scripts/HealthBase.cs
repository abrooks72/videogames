using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBase : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header(" -- Health -- ")]
    [SerializeField] private float maxHealth = 15f;
    [SerializeField] private float currentHealth;
    private float netHealth = 0f;
    [SerializeField] private Image HealthBar;
    [SerializeField] private GameObject Graphic;
    [Header(" -- Death --")]
    private int runtimeEffect = 0;
    [SerializeField] private GameObject[] explosion_Death = null;
    [SerializeField] private float timeDeath = 2f;
    [SerializeField] private float timerDeath = 0;
    [SerializeField] private bool Death = false;
    [SerializeField] GameObject DeathMenu;


    public event Action<float, float> OnHealthAmountChanged = delegate { };


    public bool GetDeath
    { get { return Death; } set { Death = value; } }

    //
    private void Start()
    {

        //  Debug.LogError("1:" + currentHealth);
        currentHealth = maxHealth;
        //   Debug.LogError("2:" + currentHealth);
        ModifyHealthDisplayNumber(currentHealth, maxHealth);
        Death = false;


    }


    public void ModifyHealthDisplayNumber(float health_current, float health_total)
    {
        OnHealthAmountChanged(health_current, health_total);
        if (HealthBar != null)
        {
            HealthBar.fillAmount = currentHealth / maxHealth;
        }
    }



    void Update()
    {
        if (!photonView.IsMine)
        {
            currentHealth = netHealth;
            if (HealthBar != null)
            {
                HealthBar.fillAmount = currentHealth / maxHealth;
            }
        }
        if (photonView.IsMine)
        {
           // netHealth = currentHealth;
            if (HealthBar != null)
            {
                HealthBar.fillAmount = currentHealth / maxHealth;
            }
            if(Input.GetKeyDown(KeyCode.H))
            {
                if(gameObject.tag == "Player")
                    Hit(5f);

            }
          
        }
        //
        CoolDownWaitDeath(timeDeath);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(Death);
        }
        else if (stream.IsReading)
        {
            netHealth = (float)stream.ReceiveNext();
            Death = (bool)stream.ReceiveNext();
        }
    }

    //
    [PunRPC]
    public void Hit(float damage)   // override 
    {
        // if (GetComponent<PhotonView>().IsMine)
        // {
        if (currentHealth > 0)
        {

            ModifyHealth(damage);
            //GetComponent<PhotonView>().RPC("ModifyHealth",RpcTarget.All,damage);
            ModifyHealthDisplayNumber(currentHealth, maxHealth);

            // check status damage 
            if (currentHealth <= 0)
            {

                Death = true;
                // StartCoroutine(CoolDownWaitDeath(timeDeath));
                GameObject explosionObj1 = Instantiate(explosion_Death[0], transform.position, transform.rotation, transform);
                Destroy(explosionObj1, timeDeath / 2f);               

            }
            //    }

        }

    }

    //[PunRPC]
    public void ModifyHealth(float amount)
    {
        currentHealth -= amount;

    }

    private void CoolDownWaitDeath(float time)
    {
        if(Death && runtimeEffect < 1)
        {
            timerDeath += Time.deltaTime;
            if (timerDeath >= timeDeath)
            {
                runtimeEffect++;
                GameObject explosionObj2 = Instantiate(explosion_Death[1]);

                explosionObj2.transform.position = transform.position;

                Destroy(explosionObj2, timeDeath);

                // if (DeathMenu)
                // {
                //     DeathMenu.SetActive(true);
                //     Time.timeScale = 0.0f;

                // }
                // else
                // {
                //     //Destroy(gameObject);
                //     GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered, null);
                // }
                if (gameObject.tag == "Player")
                {
                    Graphic.SetActive(false);
                    Invoke("LeaveRoom", 2f);
                }
                else
                {                    
                        GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered, null);
                }


            }

        }
       
    }
    // [PunRPC]
    public void LeaveRoom()
    {
        if (photonView.IsMine)
        {
            GameNetManager.instance.LeaveRoom();
        }
    }

    // private IEnumerator CoolDownWaitDeath(float time)
    // {
    //     if (Death)
    //     {

    //         GameObject explosionObj1 = Instantiate(explosion_Death[0], transform.position, transform.rotation, transform);
    //         Destroy(explosionObj1, timeDeath / 2f);

    //         yield return new WaitForSeconds(time);


    //         GameObject explosionObj2 = Instantiate(explosion_Death[1]);

    //         explosionObj2.transform.position = transform.position;

    //         Destroy(explosionObj2, timeDeath);

    //         // if (DeathMenu)
    //         // {
    //         //     DeathMenu.SetActive(true);
    //         //     Time.timeScale = 0.0f;

    //         // }
    //         // else
    //         // {
    //         //     //Destroy(gameObject);
    //         //     GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered, null);
    //         // }
    //         if (gameObject.tag == "Player")
    //         {
    //             Graphic.SetActive(false);
    //             Invoke("LeaveRoom", 2f);
    //         }
    //         else
    //         {
    //             GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered, null);
    //         }


    //         Death = false;

    //     }

    // }
    // // [PunRPC]
    // public void LeaveRoom()
    // {
    //     if (photonView.IsMine)
    //     {
    //         GameNetManager.instance.LeaveRoom();
    //     }
    // }

    public void DeathMenuPlayAgain()
    {
        //Destroy(gameObject);
        // GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered, null);
        DeathMenu.SetActive(false);
        Time.timeScale = 1f;
        print("reload current scence");
        if (photonView.IsMine)
        {
            GameNetManager.instance.LeaveRoom();
        }



    }

    [PunRPC]
    private void DestroyObject()
    {
        Graphic.SetActive(false);
        Destroy(gameObject, 1f);
    }
}
