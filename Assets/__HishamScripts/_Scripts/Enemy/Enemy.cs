// Hisham
/*using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Chase, Patrol
    }

    NavMeshAgent agent;
    Transform target;

    public EnemyState currentState;
    public Transform[] patrolPoints;
    public Transform playerTransform;
    public float distThreshold = 0.2f;
    
    private int pathIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length <= 0)
        {
            Debug.LogError("No patrol points assigned to " + gameObject.name);
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player transform not assigned to " + gameObject.name);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == EnemyState.Patrol)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Chase)
        {
            Chase();
        }

        if (!target) throw new System.Exception("Target is not assigned for " + gameObject.name);

        agent.SetDestination(target.position);
    }

    void Patrol()
    {
        if (target == playerTransform) target = patrolPoints[pathIndex];
        if (agent.remainingDistance < distThreshold)
        {
            pathIndex = (pathIndex + 1) % patrolPoints.Length;
            target = patrolPoints[pathIndex];
        }
    }

    void Chase()
    {
        if (!playerTransform) return;
        
        target = playerTransform;
    }
}
*/