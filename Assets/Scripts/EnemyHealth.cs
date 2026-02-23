using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    private Renderer rend;
    private Color originalColor;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        rend = GetComponentInChildren<Renderer>(); 
        originalColor = rend.material.color;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage! Health is now : {currentHealth}");

        if (rb != null)
        {
            Vector3 pushDirection = transform.position - Camera.main.transform.position;
            pushDirection.y = 0.2f; // A little lift
            rb.AddForce(pushDirection.normalized * 5f, ForceMode.Impulse);
        }

        // Flash red for visual feedback
        rend.material.color = Color.red;
        Invoke(nameof(ResetColor), 0.15f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        rend.material.color = originalColor;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated!");
        Destroy(gameObject);
    }
}
