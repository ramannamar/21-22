using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent enemy;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public BonusCheck bonusCheck;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float sightRange;
    public bool playerInSightRange;

    public bool escape = false;
    

    private void Awake()
    {
        player = GameObject.Find("Player").transform;    
        enemy = GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange) Patroling();
        if (playerInSightRange) ChasePlayer();
        SetEscape();
        Escape();
        
    }

    public void SetEscape()
    {         
        if (bonusCheck.isActive == true)
        {
            escape = true;
        }
        else 
        {
            escape = false;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            enemy.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        enemy.SetDestination(player.position);
    }

    private void Escape()
    {
        if (escape == true)
        {
            Vector3 direction = transform.position - player.position;

            Vector3 newPosition = transform.position + direction;

            enemy.SetDestination(newPosition);
        }
        else
        {
            ChasePlayer();
        }
    }
}
