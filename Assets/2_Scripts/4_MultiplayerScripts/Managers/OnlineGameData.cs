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

    [SerializeField] private string playerName;

    [SerializeField] private List <OnlinePlayerData> onlinePlayersConnected = new List<OnlinePlayerData>(); 
    public List<OnlinePlayerData> OnlinePlayersConnected => onlinePlayersConnected;

    public Action OnPlayersChanged;

    private void Awake()
    {
        GameServiceLocator.RegisterService<OnlineGameData>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<OnlineGameData>();
        UnsubscribeNetworkCallbacks();
    }
    private void UnsubscribeNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= PlayerDisconnected;
    }

    public override void OnNetworkSpawn()
    {
        onlinePlayersConnected = new List<OnlinePlayerData>();
        NetworkManager.Singleton.OnClientConnectedCallback +=  PlayerConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        UnsubscribeNetworkCallbacks();
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
        if (NetworkManager.Singleton.IsListening)
        {
            UpdatePlayersConnectedOnClientsRpc(onlinePlayersConnected.ToArray());
        }
        OnPlayersChanged?.Invoke();
    }

    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    public void UpdatePlayersConnectedOnClientsRpc(OnlinePlayerData[] playersConnected)
    {
        // TODO: esto se puede optimizar, no hace falta enviar toda la lista cada vez, pero bueno, es un ejemplo
        onlinePlayersConnected = playersConnected.ToList();
        OnPlayersChanged?.Invoke();

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

    public bool CheckAllPlayersReady()
    {
        foreach (var player in onlinePlayersConnected)
        {
            if (!player.isReady)
            {
                return false;
            }
        }
        return true;
    }


    
}