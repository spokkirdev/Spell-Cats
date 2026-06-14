using Unity.Netcode;
using UnityEngine;

public class NetworkEnemy : NetworkBehaviour
{
    private NetworkPatrollingEnemy _patrollingEnemy;
    private Sensor[] _sensors;
    private Attack[] _attacks;

    private Transform _currentTarget;

    [SerializeField] private float loseTargetDelay = 1f;
    private float _lastTimeSeen;

    private void Awake()
    {
        _patrollingEnemy = GetComponent<NetworkPatrollingEnemy>();
        _sensors = GetComponentsInChildren<Sensor>();
        _attacks = GetComponentsInChildren<Attack>();
    }

    private void Update()
    {
        if (!IsServer) return;
        Transform detectedTarget = GetTargetFromSensors();

        if (detectedTarget != null)
        {
            _currentTarget = detectedTarget;
            _lastTimeSeen = Time.time;

            HandleChaseAndAttack();
        }
        else
        {
            HandleLoseTarget();
        }
    }


    private Transform GetTargetFromSensors()
    {
        foreach (var sensor in _sensors)
        {
            if (sensor.Detect(out Transform target))
            {
                return target;
            }
        }

        return null;
    }


    private void HandleChaseAndAttack()
    {
        if (_currentTarget == null) return;

        if (TryAttack(_currentTarget))
        {
            return; 
        }

        _patrollingEnemy.Chase(_currentTarget);
    }

    private bool TryAttack(Transform target)
    {
        foreach (var attack in _attacks)
        {
            if (attack.CanAttack(target))
            {
                attack.PerformAttack(target);
                return true;
            }
        }

        return false;
    }

    private void HandleLoseTarget()
    {
        if (_currentTarget == null)
            return;

        if (Time.time > _lastTimeSeen + loseTargetDelay)
        {
            _patrollingEnemy.StopChasing();
            _currentTarget = null;
        }
    }
}