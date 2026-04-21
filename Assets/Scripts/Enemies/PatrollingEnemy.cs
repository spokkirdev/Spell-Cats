using UnityEngine;
using System.Collections;

public enum EnemyState
{
    Waiting,
    Patrolling,
    Chasing,
    Attacking
}

public class PatrollingEnemy : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject patrolPointsParent;

    [Header("Settings")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float stopDistance = 0.5f;


    private Transform[] _patrolPoints;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private int _currentPatrolIndex;
    private EnemyState _currentState;



    private void Start()
    {
        if (patrolPointsParent == null)
        {
            return;
        }

        _patrolPoints = new Transform[patrolPointsParent.transform.childCount];

        for (int i = 0; i < patrolPointsParent.transform.childCount; i++)
        {
            _patrolPoints[i] = patrolPointsParent.transform.GetChild(i);
        }
        _currentPatrolIndex = 0;
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GoToNextPatrolPoint();

    }


    private void Update()
    {
        Patrol();
    }


    private void Patrol()
    {
        if (_currentState != EnemyState.Patrolling) return;
        Debug.Log(_navMeshAgent.remainingDistance < stopDistance);
        Debug.Log(!_navMeshAgent.pathPending);
        if (_navMeshAgent.remainingDistance < stopDistance && !_navMeshAgent.pathPending)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _currentState = EnemyState.Waiting;
        _navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(waitTime); // Wait for the specified time at the patrol point
        _navMeshAgent.isStopped = false;
        GoToNextPatrolPoint();
        _currentState = EnemyState.Patrolling;
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;
        _currentState = EnemyState.Patrolling;
        _navMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // Loop back to the first point
    }
}
