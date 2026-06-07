using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class OnlinePlayerData : INetworkSerializable
{
    public ulong ClientId;
    public string PlayerName;
    public bool isReady = false;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref isReady);
    }

}

public class OnlineGameData : NetworkBehaviour
{
    private static OnlineGameData _singleton;
    public static OnlineGameData Singleton => _singleton;

    [SerializeField] private string playerName;

    [SerializeField] private List <OnlinePlayerData> onlinePlayersConnected = new List<OnlinePlayerData>(); 
    public List<OnlinePlayerData> OnlinePlayersConnected => onlinePlayersConnected;

    public Action NewPlayerOnList;

    private void Awake()
    {
        if (_singleton == null)
        {
            _singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback +=  PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        onlinePlayersConnected = new List<OnlinePlayerData>();
    }

    private void PlayerConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            OnlinePlayerData newPlayer = new OnlinePlayerData();
            newPlayer.ClientId = clientId;
            newPlayer.PlayerName = playerName;
            newPlayer.isReady = false;

            AddPlayerConnectedRpc(newPlayer);
        }

    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddPlayerConnectedRpc(OnlinePlayerData playerData)
    {
        onlinePlayersConnected.Add(playerData);
        PlayersListChanged();
    }

    private void PlayersListChanged()
    {
        UpdatePlayersConnectedOnClientsRpc(onlinePlayersConnected.ToArray());
        NewPlayerOnList?.Invoke();
    }

    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    public void UpdatePlayersConnectedOnClientsRpc(OnlinePlayerData[] playersConnected)
    {
        // TODO: esto se puede optimizar, no hace falta enviar toda la lista cada vez, pero bueno, es un ejemplo
        onlinePlayersConnected = playersConnected.ToList();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
        }
    }

    private void PlayerDisconnected(ulong clientId)
    {

        if (NetworkManager.Singleton.IsHost)
        {

            for (int i = 0; i < onlinePlayersConnected.Count; i++)
            {
                if (onlinePlayersConnected[i].ClientId == clientId)
                {
                    onlinePlayersConnected.RemoveAt(i);
                    PlayersListChanged();
                    break;
                }
            }
        }
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            onlinePlayersConnected.Clear();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    internal void UpdatePlayerReadyServerRpc(ulong localClientId)
    {
        for (int i = 0; i < onlinePlayersConnected.Count; i++)
        {
            if (onlinePlayersConnected[i].ClientId == localClientId)
            {
                onlinePlayersConnected[i].isReady = !onlinePlayersConnected[i].isReady;
                PlayersListChanged();
                break;
            }
        }
    }

    public string PlayerName
    {
        get {return playerName;}
        set {playerName = value;}
    }


    
}