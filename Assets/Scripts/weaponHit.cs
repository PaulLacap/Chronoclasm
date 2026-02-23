using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class WeaponHit : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 20f;             // Damage dealt by this weapon
    public string targetTag = "Goblin";    // Tag of objects this weapon can hit
    public float swingDuration = 0.5f;     // Duration of the swing

    private Collider weaponCollider;
    private HashSet<GoblinController> hitGoblins = new HashSet<GoblinController>();
    private bool isSwinging = false;

    private PlayerInput playerInput;
    private InputAction attackAction;

    private void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider == null)
        {
            Debug.LogError("WeaponHit requires a Collider on the same GameObject!");
        }
        weaponCollider.enabled = false; // Start disabled

        // Get the PlayerInput component
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("No PlayerInput found in the scene!");
        }
        else
        {
            // Grab the Attack action
            attackAction = playerInput.actions["Attack"];
            if (attackAction == null)
            {
                Debug.LogError("Attack action not found in the Input Action Asset!");
            }
            else
            {
                attackAction.performed += ctx => StartSwing();
            }
        }
    }

    private void StartSwing()
    {
        if (!isSwinging)
        {
            StartCoroutine(SwingWeapon());
        }
    }

    private IEnumerator SwingWeapon()
    {
        isSwinging = true;
        hitGoblins.Clear();
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(swingDuration);

        weaponCollider.enabled = false;
        isSwinging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSwinging) return;

        if (other.CompareTag(targetTag))
        {
            GoblinController goblin = other.GetComponent<GoblinController>();
            if (goblin != null && !hitGoblins.Contains(goblin))
            {
                goblin.TakeDamage(damage);
                hitGoblins.Add(goblin);
                Debug.Log("Hit " + goblin.name + " for " + damage + " damage.");
            }
        }
    }
}