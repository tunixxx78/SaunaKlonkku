using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;


public class PhotonPublicRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //room info
    public static PhotonPublicRoom room;
    private PhotonView pV;

    public bool isGameLoaded;
    public int currentScene;

    //Player Info
    private Player[] photonPlayers;
    public int playersInRoom, myNumberInRoom, PlayerInGame;

    //Delay Start
    private bool readyToCount, readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers, atMaxPlayer, timeToStart;

    private void Awake()
    {
        if (PhotonPublicRoom.room == null)
        {
            PhotonPublicRoom.room = this;
        }
        else
        {
            if (PhotonPublicRoom.room != this)
            {
                Destroy(PhotonPublicRoom.room.gameObject);
                PhotonPublicRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start()
    {
        pV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    private void Update()
    {
        if (MultiplayerSettings.multiplayerSetting.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }

                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We Are in room!");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        if (MultiplayerSettings.multiplayerSetting.delayStart)
        {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSetting.maxPlayers + ")");

            PhotonPublicLobby.lobby.playerAmountText.text = playersInRoom + " : " + MultiplayerSettings.multiplayerSetting.maxPlayers;

            if (playersInRoom > MultiplayerSettings.multiplayerSetting.minAmountOfPlayersToStart - 1)
            {
                readyToCount = true;
                PhotonPublicLobby.lobby.enteringGameText.text = "Get ready to start in ... ";
                PhotonPublicLobby.lobby.timerText.SetActive(true);
                PhotonPublicLobby.lobby.canStartTimer = true;

            }
            if (playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
            {
                readyToStart = true;

                if (!PhotonNetwork.IsMasterClient)
                    return;

                PhotonNetwork.CurrentRoom.IsOpen = false;

                StartGame();
            }
        }
        else
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(" New player has joined the room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        if (MultiplayerSettings.multiplayerSetting.delayStart)
        {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSetting.maxPlayers + ")");

            PhotonPublicLobby.lobby.playerAmountText.text = playersInRoom + " : " + MultiplayerSettings.multiplayerSetting.maxPlayers;

            if (playersInRoom > MultiplayerSettings.multiplayerSetting.minAmountOfPlayersToStart - 1)
            {
                readyToCount = true;
                PhotonPublicLobby.lobby.enteringGameText.text = "Get ready to start in ... ";
                PhotonPublicLobby.lobby.timerText.SetActive(true);
                PhotonPublicLobby.lobby.canStartTimer = true;

            }
            if (playersInRoom == MultiplayerSettings.multiplayerSetting.maxPlayers)
            {
                readyToStart = true;

                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;

                StartGame();
            }
        }
    }

    public void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (MultiplayerSettings.multiplayerSetting.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSetting.multiplayerScene);
    }

    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;

    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiplayerSetting.multiplayerScene)
        {
            isGameLoaded = true;

            if (MultiplayerSettings.multiplayerSetting.delayStart)
            {
                pV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        PlayerInGame++;

        if (PlayerInGame == PhotonNetwork.PlayerList.Length)
        {
            pV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("photonPrefabs", "PLR"), transform.position, Quaternion.identity, 0);
    }
}
