using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;


public enum EnemyState
{
    Waiting,
    Patrolling,
    Chasing,
    Attacking
}


public class NetworkPatrollingEnemy : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject patrolPointsParent;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float stopDistance = 0.5f;


    private Transform[] _patrolPoints;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private int _currentPatrolIndex;
    private NetworkVariable<EnemyState> _currentNetworkState = new NetworkVariable<EnemyState>(
        EnemyState.Waiting,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


    public EnemyState GetState() => _currentNetworkState.Value;

    private void Initialize()
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

    public void SetPatrolPointsParent(GameObject newPatrolPointsParent)
    {
        patrolPointsParent = newPatrolPointsParent;
        Initialize();
    }


    private void Update()
    {
        UpdateAnimator();
        if (!IsServer) return;
        Patrol();
    }

    private void UpdateAnimator()
    {
        switch (_currentNetworkState.Value)
        {
            case EnemyState.Waiting:
                // Set waiting animation
                animator.SetBool("IsMoving", false);
                break;
            case EnemyState.Patrolling:
                // Set patrolling animation
                animator.SetBool("IsMoving", true);
                animator.SetBool("Chasing", false);
                break;
            case EnemyState.Chasing:
                // Set chasing animation
                animator.SetBool("Chasing", true);
                break;
        }
        
    }

    private void Patrol()
    {
        if (_currentNetworkState.Value != EnemyState.Patrolling) return;
        if (_navMeshAgent.remainingDistance < stopDistance && !_navMeshAgent.pathPending)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _currentNetworkState.Value = EnemyState.Waiting;
        _navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(waitTime); // Wait for the specified time at the patrol point
        _navMeshAgent.isStopped = false;
        GoToNextPatrolPoint();
        _currentNetworkState.Value = EnemyState.Patrolling;
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;
        _currentNetworkState.Value = EnemyState.Patrolling;
        _navMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // Loop back to the first point
    }

    public void Chase(Transform chasePosition)
    {
        _currentNetworkState.Value = EnemyState.Chasing;

        _navMeshAgent.SetDestination(chasePosition.position);
        _navMeshAgent.speed = chaseSpeed;
    }

    public void StopChasing()
    {
        _navMeshAgent.speed = patrolSpeed;
        _currentNetworkState.Value = EnemyState.Patrolling;
        GoToNextPatrolPoint();
    }

    public void PerformAttackAnimation()
    {
        if (_currentNetworkState.Value != EnemyState.Attacking)
        {
            _currentNetworkState.Value = EnemyState.Attacking;
            animator.SetBool("Attacking", true);
            StartCoroutine(ResetAttackAnimation());
        }
    }

    private IEnumerator ResetAttackAnimation()
    {

        yield return new WaitForSeconds(1f); 
        animator.SetBool("Attacking", false);
        _currentNetworkState.Value = EnemyState.Chasing; 
        
    }

}
