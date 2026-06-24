using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkHealthComponent : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;


    [SerializeField] private NetworkVariable<float> _currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,  
        NetworkVariableWritePermission.Server   
    );

    public float CurrentHealth => _currentHealth.Value;
    public float MaxHealth => maxHealth;

    public event Action<float, float> OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        _currentHealth.OnValueChanged += HandleHealthChanged;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"[NetworkHealthComponent] Registering service for clientId: {NetworkManager.Singleton.LocalClientId}");
            GameServiceLocator.RegisterService<NetworkHealthComponent>(this);
            GameServiceLocator.GetService<UIManager>().InitializeHealthBar();
        }
        if (IsServer)
        {
            _currentHealth.Value = maxHealth;
        }
    }

    public override void OnNetworkDespawn()
    {
        _currentHealth.OnValueChanged -= HandleHealthChanged;
        if (NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"[NetworkHealthComponent] Unregistering service for clientId: {NetworkManager.Singleton.LocalClientId}");
            GameServiceLocator.UnregisterService<NetworkHealthComponent>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer) return;
        _currentHealth.Value = Mathf.Max(_currentHealth.Value - damage, 0f);

        if (_currentHealth.Value <= 0f)
        {
            OnDeath();
        }
    }
    

    private void HandleHealthChanged(float previous, float current)
    {
        OnHealthChanged?.Invoke(previous, current);
    }

    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Implement death logic here (e.g., destroy the object, play animation, etc.)

        if (!IsServer)
            return;

        RespawnCoroutine();
    }

    private void RespawnCoroutine()
    {
        ulong ownerId = OwnerClientId;

        NetworkObject.Despawn(true);


        GameServiceLocator.GetService<NetworkPlayerSpawner>()
            .SpawnPlayer(ownerId);
    }

    [ServerRpc]
    public void RequestRespawnServerRpc()
    {
        Debug.Log($"[NetworkHealthComponent] Respawn requested by client {OwnerClientId}");
        RespawnCoroutine();
    }

}
