using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{

    public GameObject EnterGamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;


    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // Set up Panel UI
        EnterGamePanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }


    #region Photon Callbacks

    public override void OnConnected()       // use to check connection
    {
        print("Connected to Internet!");
    }

    public override void OnConnectedToMaster() // use to check connection 
    {
        print("'" + PhotonNetwork.NickName + "'" + " connected to photon server!");
        // if the connection is successful, then close the connectionStatus and go to the lobby room
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        print("Join Random Room Message: " + message);
        // if failed on Join Random Room  then
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.NickName + " joined Room " + PhotonNetwork.CurrentRoom.Name);
        //
        PhotonNetwork.LoadLevel("pgc_mesh_05");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " \n You are the player #" + +PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion

    //------------------------------------------------------------
    // check photon is connected? Then check who is connected
    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = "v1";
            PhotonNetwork.ConnectUsingSettings();
            //after the player input the player name and press button connect
            EnterGamePanel.SetActive(false);
            ConnectionStatusPanel.SetActive(true);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Random " + UnityEngine.Random.Range(0, 100);

        RoomOptions roomOption = new RoomOptions();   
        roomOption.IsOpen = true;
        roomOption.IsVisible = true;
        roomOption.MaxPlayers = 20;      
        

        PhotonNetwork.CreateRoom(randomRoomName, roomOption);

    }






}
