using UnityEngine;
using Photon.Pun;

public class PlayerNameInputManager : MonoBehaviour
{
   public void SetPlayerName(string playername)
   {
       if(string.IsNullOrEmpty(playername))
       {
           print("Player name is empty");
           return;
       }

       PhotonNetwork.NickName = playername;
   }


}
