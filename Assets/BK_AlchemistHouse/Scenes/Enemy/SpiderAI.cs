using UnityEngine;
using UnityEngine.AI;

public class SpiderAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Distances")]
    public float walkDistance = 12f;    
    public float runDistance = 6f;       // running
    public float attackDistance = 2f;    // attacking

    [Header("Random Walk Settings")]
    public float wanderRadius = 8f;
    public float wanderInterval = 3f;
    private float wanderTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
{
    if (player == null || !agent.isOnNavMesh) return; // <<< important

    float dist = Vector3.Distance(transform.position, player.position);

    bool isWalking = false;
    bool isRunning = false;
    bool isAttacking = false;

    if (dist < attackDistance)
    {
        agent.isStopped = true;
        isAttacking = true;
    }
    else if (dist < runDistance)
    {
        agent.isStopped = false;
        agent.speed = 6f;
        agent.SetDestination(player.position);
        isRunning = true;
    }
    else if (dist < walkDistance)
    {
        agent.isStopped = false;
        agent.speed = 3f;
        agent.SetDestination(player.position);
        isWalking = true;
    }
    else
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            Vector3 newPos = RandomNavmeshPoint(transform.position, wanderRadius);
            if (agent.isOnNavMesh) agent.SetDestination(newPos);
            wanderTimer = wanderInterval;
        }

        isWalking = agent.velocity.magnitude > 0.1f;
    }

    if (agent.isStopped || agent.velocity.magnitude < 0.05f)
    {
        isWalking = false;
        isRunning = false;
    }

    animator.SetBool("IsWalking", isWalking);
    animator.SetBool("IsRunning", isRunning);
    animator.SetBool("IsAttacking", isAttacking);
}

    Vector3 RandomNavmeshPoint(Vector3 origin, float dist)
    {
        Vector3 rand = origin + Random.insideUnitSphere * dist;
        rand.y = origin.y;

        NavMeshHit hit;
        NavMesh.SamplePosition(rand, out hit, dist, NavMesh.AllAreas);

        return hit.position;
    }
}
