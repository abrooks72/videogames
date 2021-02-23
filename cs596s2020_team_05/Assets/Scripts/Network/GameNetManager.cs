using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameNetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;

    public static GameNetManager instance;

    private void Awake() // Singoton
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {       

        if (PhotonNetwork.IsConnected)
        {
            if (playerPrefab != null)
            {
                // make  position spawn // center is 0,>1,0
                int random = UnityEngine.Random.Range(-2,2);
                Vector3 p = new Vector3(86,2f,120);
                Vector3 spawnPoint = p +  new Vector3(random,0,random) ;              
                // this one instantiate all clients joining the room 
               PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);              
            }
        }
    } 
    //
    public override void OnJoinedRoom()  // it's you
    {
        print(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // your neighbors
    {
        print(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("GameLaucherScene");
    }
    //----------------
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

}
