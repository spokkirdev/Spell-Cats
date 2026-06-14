using System;
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
        if (IsServer)
        {
            _currentHealth.Value = maxHealth;
        }
    }

    public override void OnNetworkDespawn()
    {
        _currentHealth.OnValueChanged -= HandleHealthChanged;
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
    }
}
