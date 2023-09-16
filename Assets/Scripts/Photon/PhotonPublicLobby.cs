using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonPublicLobby : MonoBehaviourPunCallbacks
{
    public static PhotonPublicLobby lobby;
    private PhotonView pV;

    public GameObject timerText;

    [SerializeField] float waitTime, currentTime;
    public TMP_Text TimerText, enteringGameText, playerAmountText;
    public bool canStartTimer = false;

    private void Awake()
    {
        lobby = this;
    }

    private void Start()
    {
        pV = GetComponent<PhotonView>();
        PhotonNetwork.ConnectUsingSettings();
        TimerText.text = waitTime.ToString();
        waitTime = PhotonPublicRoom.room.startingTime;
        currentTime = waitTime;
    }

    private void Update()
    {

        playerAmountText.text = PhotonPublicRoom.room.playersInRoom + " : " + MultiplayerSettings.multiplayerSetting.maxPlayers;

        if (canStartTimer)
        {
            currentTime -= 1 * Time.deltaTime;

            if (currentTime <= 0)
            {
                canStartTimer = false;
                TimerText.text = "0";

                PhotonPublicRoom.room.StartGame();
            }
        }

        string tempTimer = string.Format("{0:00}", currentTime);
        TimerText.text = tempTimer;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to photon master server!");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnJoinSauna()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join but failed, there must not be oppen games available");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating new room!");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSetting.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create new room but failed, there must already be a room with same name!");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        canStartTimer = false;
        currentTime = waitTime;
        timerText.SetActive(false);
        PhotonNetwork.LeaveRoom();
        
    }

}
