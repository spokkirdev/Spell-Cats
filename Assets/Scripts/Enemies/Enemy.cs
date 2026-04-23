using UnityEngine;

public class Enemy : MonoBehaviour
{

    private PatrollingEnemy _patrollingEnemy;
    private Sensor[] _sensors;

    private Transform _currentTarget;

    private void Awake()
    {
        _patrollingEnemy = GetComponent<PatrollingEnemy>();
        _sensors = GetComponentsInChildren<Sensor>();
    }

    private void Update()
    {

        foreach (var sensor in _sensors)
        {
            if (sensor.Detect(out Transform target))
            {
                _patrollingEnemy.Chase(target);
                _currentTarget = target;
                return;
            }
        }

        if (_currentTarget != null)
        {
            _patrollingEnemy.StopChasing();
            _currentTarget = null;
        }
    }

}
