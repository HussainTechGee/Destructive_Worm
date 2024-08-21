using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FusionConnection : FusionMonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runnerIstance;
    public static FusionConnection instance;
    public NetworkObject LocalPlayer;
    public Dictionary<string, GameObject> sessionListUiDictionary = new Dictionary<string, GameObject>();
    PlayerRef myPlayer;
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    async Task Start()
    {
        runnerIstance = gameObject.GetComponent<NetworkRunner>();
        if(runnerIstance==null)
        {
            runnerIstance = gameObject.AddComponent<NetworkRunner>();
        }
        await joinLobby();
    }
    async Task joinLobby()
    {
        if(runnerIstance!=null)
        {
            var result = await runnerIstance.JoinSessionLobby(SessionLobby.Shared);

            if (result.Ok)
            {
                Debug.Log("Enter into Lobby");
                LobbyUI.instance.OpenLobbyPanel();
                // all good
            }
            else
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }
    }
    public void CreateRandomSession()
    {
        //GameUIScript.instance.LoadingPanel.SetActive(true);
        string roomName = "Room_"+ UnityEngine.Random.Range(1000, 9999);
        runnerIstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = 4,

        });
    }
    public bool isMasterClient()
    {
        bool isMaster=false;
        if(myPlayer.PlayerId==1)
        {
            isMaster = true;
        }
        return isMaster;
           
    }
    public void JoinSession(string name)
    {

        runnerIstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
        });
        
    }
    public GameObject CreatePlayerOnGameScene()
    {
        LocalPlayer = runnerIstance.Spawn(BattleSystem.instance.playerPrefab, BattleSystem.instance.playerBattleStation[((myPlayer.PlayerId-1) % BattleSystem.instance.playerBattleStation.Length)].position, Quaternion.identity, myPlayer);
        return LocalPlayer.gameObject;
    }
    public void DissconctPlayer()
    {
       // runnerIstance.Despawn(runnerIstance.GetPlayerObject(runnerIstance.LocalPlayer));
        runnerIstance.Shutdown(true, ShutdownReason.Ok);
    }
    void CompareLists(List<SessionInfo> sessionList)
    {
        foreach(SessionInfo session in sessionList)
        {
            if(sessionListUiDictionary.ContainsKey(session.Name))
            {
                UpdateEnteryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }
        }
    }
    void CreateEntryUI(SessionInfo session)
    {
        GameObject entryObj=LobbyUI.instance.SessionListEntryAdd();
        sessionListUiDictionary.Add(session.Name,entryObj);
        entryObj.GetComponent<SessionListEntry>().Setup(session.Name, session.PlayerCount + "/" + session.MaxPlayers, session.IsOpen);
        entryObj.SetActive(session.IsVisible);
    }
    void UpdateEnteryUI(SessionInfo session)
    {
        sessionListUiDictionary.TryGetValue(session.Name, out GameObject entryObj);
        if(session.PlayerCount==session.MaxPlayers)
        {
            session.IsOpen = false;
        }
        else
        {
            session.IsOpen = true;
        }
        entryObj.GetComponent<SessionListEntry>().Setup(session.Name, session.PlayerCount + "/" + session.MaxPlayers, session.IsOpen);
        entryObj.SetActive(session.IsVisible);
    }
    void DeleteOldSessionsFromList(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject deleteObj=null;
        foreach(KeyValuePair<string,GameObject> kvp in sessionListUiDictionary)
        {
            string sessionKey = kvp.Key;
            foreach(SessionInfo sessionInfo in sessionList)
            {
                if(sessionInfo.Name==sessionKey)
                {
                    isContained = true;
                    break;
                }
            }
            if(!isContained)
            {
                deleteObj = kvp.Value;
                sessionListUiDictionary.Remove(sessionKey);
                Destroy(deleteObj);
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Sever Connected!");
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Sever Failed Connected!");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Sever Request Connected!");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Sever Authentication Connected!");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Sever Disconected!");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
         
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
         
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
         
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
         
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
         
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //When Local Player jopin the Session
         if(player==runnerIstance.LocalPlayer)
        {
            LobbyUI.instance.OpenWaitingPanel();
            LobbyUI.instance.WaitingPanelUpdate(player.PlayerId);
            myPlayer = player;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
         
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
         
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
         
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
         
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
         
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //Delete all the old Session Entries
        DeleteOldSessionsFromList(sessionList);
        
        //Compare the Session List to update and Create Entries;
        CompareLists(sessionList);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        GameManager.instance.GoToScene(0);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
         
    }
}
