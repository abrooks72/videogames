using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("-- Moving --")]
    [SerializeField] private float rotatingSpeed = 4f;
    [SerializeField] private float velocitySpeed = 3.5f;
    private float _xMov = 0;
    private float _zMov = 0;

    [SerializeField] private GameObject modelPlayer;
    [SerializeField] private GameObject[] weaponPlayerArr;
    public Rigidbody rigidbodyPlayer;
    private Animator animator = null;
    private Radar _playerRadar;

    public GameObject GetModelPlayer
    { get { return modelPlayer; } set { modelPlayer = value; } }

    public GameObject[] GetWeaponPlayerArr
    { get { return weaponPlayerArr; } set { weaponPlayerArr = value; } }
    //-------------
    private Vector3 _playerPosition;
    private Quaternion _playerRotation;
    private Quaternion _playerRotationPart;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Quaternion modelplayerRotation;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyPlayer = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        _playerRadar = GetComponent<Radar>();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           // stream.SendNext(_xMov);
            //stream.SendNext(_zMov);
            stream.SendNext(modelPlayer.transform.rotation);

            stream.SendNext(rigidbodyPlayer.position);
            stream.SendNext(rigidbodyPlayer.rotation);
            stream.SendNext(rigidbodyPlayer.velocity);
        }
        else if (stream.IsReading)
        {
            //_xMov = (float)stream.ReceiveNext();
           // _zMov = (float)stream.ReceiveNext();
            modelPlayer.transform.rotation = (Quaternion)stream.ReceiveNext();
           

            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            rigidbodyPlayer.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            rigidbodyPlayer.position += rigidbodyPlayer.velocity * lag;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {            
            rigidbodyPlayer.rotation = Quaternion.RotateTowards(modelPlayer.transform.rotation, modelplayerRotation, Time.fixedDeltaTime * 100.0f);

        }
    }



    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rigidbodyPlayer.position = Vector3.MoveTowards(rigidbodyPlayer.position, networkPosition, Time.fixedDeltaTime);
            rigidbodyPlayer.rotation = Quaternion.RotateTowards(rigidbodyPlayer.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    //----------------------
    [PunRPC]
    public bool InputPlayerMove()
    {
        if (photonView.IsMine)
        {
            bool isMove = false;

            _xMov = Input.GetAxis("Horizontal");
            _zMov = Input.GetAxis("Vertical");
            //
            Vector3 movement = new Vector3(-_xMov, 0, -_zMov) * velocitySpeed * Time.deltaTime;
            Vector3 lookDir = new Vector3(movement.x, 0f, movement.z);
            //
            if (lookDir != Vector3.zero)
            {
                isMove = true;
                // rotation
                Quaternion rotating = Quaternion.LookRotation(lookDir);
                modelPlayer.transform.rotation = Quaternion.Lerp(modelPlayer.transform.rotation, rotating, Time.deltaTime * rotatingSpeed);

                if (!_playerRadar.GetBestTarget)
                {
                    for (int i = 0; i < weaponPlayerArr.Length; i++)
                    {
                        weaponPlayerArr[i].transform.rotation
                              = Quaternion.Lerp(weaponPlayerArr[i].transform.rotation, rotating, Time.deltaTime * rotatingSpeed);
                        // transform.rotation
                        //       = Quaternion.Lerp(transform.rotation, rotating, Time.deltaTime * rotatingSpeed);


                    }

                }

                rigidbodyPlayer.MovePosition(rigidbodyPlayer.position + movement);

            }
            else
            {
                isMove = false;
            }
            return isMove;

        }
        return false;


    }




}// End PlayerMove 
