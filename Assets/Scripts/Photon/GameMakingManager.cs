using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMakingManager : MonoBehaviour
{
    GameObject publicRoom, publicLobby, privateRoom, privateLobby;

    private void Awake()
    {
        publicRoom = GameObject.Find("PublicRoomController");
        publicLobby = GameObject.Find("PublicLobbyController");
        privateRoom = GameObject.Find("PrivateRoomController");
        privateLobby = GameObject.Find("PrivateLobbyController");
    }

    public void ChoosePublicSauna()
    {
        privateRoom.SetActive(false);
        privateLobby.SetActive(false);
    }

    public void ChoosePrivateSauna()
    {
        publicRoom.SetActive(false);
        publicLobby.SetActive(false);
    }

    public void ResetSaunaSelection()
    {
        publicRoom.SetActive(true);
        publicLobby.SetActive(true);
        privateRoom.SetActive(true);
        privateLobby.SetActive(true);
    }
}
