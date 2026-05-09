using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : BaseState<EnemyController>
{
    private NavMeshAgent _navAgent;
    private float _wanderRadius;
    private Vector3 _startPosition;

    public EnemyWanderState(EnemyController agent, Animator animator, NavMeshAgent navAgent, float wanderRadius) : base(agent, animator) 
    { 
        _navAgent = navAgent;
        _wanderRadius = wanderRadius;
        _startPosition = agent.transform.position;
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Wander State");
        animator.CrossFade(EnemyAnimIDs.Walk, crossFadeDuration);
    }

    public override void Update()
    {
        if(HasReachedDestination())
        {
            Vector3 newPos = Random.insideUnitSphere * _wanderRadius + _startPosition;
            NavMeshHit hit;
            NavMesh.SamplePosition(newPos, out hit, _wanderRadius, NavMesh.AllAreas);
            var finalPosition = hit.position;
            _navAgent.SetDestination(finalPosition);
        }
    }

    private bool HasReachedDestination()
    {
        return !_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance 
            && (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f);
    }
}