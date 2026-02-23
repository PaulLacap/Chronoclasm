using UnityEngine;
using UnityEngine.AI;

public class GoblinController : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;

    [Header("Combat Settings")]
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;

    [Header("Stats")]
    public float maxHealth = 50f;

    private float currentHealth;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isAttacking = false;

    private float distance;

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Stop slightly before attack range so we don’t clip into the player
        agent.stoppingDistance = attackRange * 0.9f;

        Debug.Log("Tracking player: " + player?.name);
    }

    void Update()
    {
        if (isDead || player == null) return;

        distance = Vector3.Distance(transform.position, player.position);

        if (distance > chaseRange)
        {
            StopMoving();
            return;
        }

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        if (isAttacking) return;

        agent.isStopped = false;
        // Continuously update destination so the goblin follows a moving player
        agent.SetDestination(player.position);

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void AttackPlayer()
    {
        // Face player
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);

        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);

        // Cooldown gate
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        animator.SetBool("IsAttacking", true);
    }

    // Call this as an Animation Event on the final frame of the attack
    public void OnAttackEnd()
    {
        isAttacking = false;
        lastAttackTime = Time.time;
        animator.SetBool("IsAttacking", false);
    }

    // Call this as an Animation Event when the weapon should hit
    public void DealDamage()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange + 0.5f)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetFloat("Speed", 0f);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Dead");
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        animator.SetBool("IsDead", true);

        Destroy(gameObject, 5f);
    }
}