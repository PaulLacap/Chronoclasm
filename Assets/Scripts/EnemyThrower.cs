using UnityEngine;
using UnityEngine.AI;

public class EnemyThrower : MonoBehaviour
{
    public Transform player;
    public Transform throwPoint;
    public GameObject projectilePrefab;

   
    public float idealDistance = 10f;
    public float retreatDistance = 6f;
    public float maxDistance = 15f;

  
    public float throwForce = 15f;
    public float throwCooldown = 2f;

    private NavMeshAgent agent;
    private float nextThrowTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 🔴 TOO CLOSE → Move Away
        if (distance < retreatDistance)
        {
            agent.isStopped = false;

            Vector3 directionAway = (transform.position - player.position).normalized;
            Vector3 newPosition = transform.position + directionAway * idealDistance;

            agent.SetDestination(newPosition);
        }
        // 🟡 TOO FAR → Move Closer
        else if (distance > maxDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        // 🟢 GOOD DISTANCE → Stop & Shoot
        else
        {
            agent.isStopped = true;
            FacePlayer();

            if (Time.time >= nextThrowTime)
            {
                ThrowProjectile();
                nextThrowTime = Time.time + throwCooldown;
            }
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                8f * Time.deltaTime
            );
        }
    }

    void ThrowProjectile()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            throwPoint.position,
            throwPoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 target = player.position + Vector3.up * 1.2f;
            Vector3 direction = (target - throwPoint.position).normalized;

            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }
    }
}