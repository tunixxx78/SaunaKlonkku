using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonPrivateLobby : MonoBehaviourPunCallbacks
{
    public static PhotonPrivateLobby lobby;

    public GameObject timerText;
    public TMP_Text TimerText, enteringGameText, playerAmountText;
    [SerializeField] float waitTime, currentTime;
    public bool canStartTimer = false;

    public string roomName, plrName;
    public int roomSize;

    public List<RoomInfo> roomListings;


    PhotonView pv;

    private void Awake()
    {
        lobby = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        pv = GetComponent<PhotonView>();
        roomSize = MultiplayerSettings.multiplayerSetting.maxPlayers;

        TimerText.text = waitTime.ToString();
        waitTime = PhotonPrivateRoom.room.startingTime;
        currentTime = waitTime;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to : " + PhotonNetwork.CloudRegion + " Server!");
        PhotonNetwork.AutomaticallySyncScene = true;


    }

    private void Update()
    {
        playerAmountText.text = PhotonPrivateRoom.room.playersInRoom + " : " + MultiplayerSettings.multiplayerSetting.maxPlayers;

        if (canStartTimer)
        {
            currentTime -= 1 * Time.deltaTime;

            if (currentTime <= 0)
            {
                canStartTimer = false;
                TimerText.text = "0";

                if (!PhotonNetwork.IsMasterClient)
                    return;

                //PhotonPrivateRoom.room.StartGame();
            }
        }

        string tempTimer = string.Format("{0:00}", currentTime);
        TimerText.text = tempTimer;
    }

    public void CreateRoom()
    {
        plrName = PhotonNetwork.NickName;

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps);

        Debug.Log("Room is created: " + roomName + roomOps);

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried create room but failed, there must be a room with a same name!!!!");
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void JoinLobbyOnClick()
    {
        PhotonNetwork.NickName = plrName;

        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        Debug.Log("NIKKI ANNETTU JA LIITYTTY LOBBYYN!");


    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnCancelButtonClicked()
    {
        canStartTimer = false;
        currentTime = waitTime;
        timerText.SetActive(false);
        PhotonNetwork.LeaveRoom();

    }
}
