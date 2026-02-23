using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class RangedSystem : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f; // time between shots
    public float recoilForce = 2f; // how far to push back

    [Header("Visuals")]
    public LineRenderer bulletTrail;
    public Transform gunTip; // where the line starts

    private float _nextFireTime = 0f;
    private StarterAssets.StarterAssetsInputs _input;
    private CharacterController _controller;
    private Camera _mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        bulletTrail.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Check Input and Cooldown
        if (_input.shoot && Time.time >= _nextFireTime)
        {
            Shoot();
            _input.shoot = false; // Reset input
        }
    }

    void Shoot()
    {
        _nextFireTime = Time.time + fireRate;

        // 1. recoil (push character backwards)
        StartCoroutine(RecoilPush());

        // 2. raycast (the aiming logic)
        Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Vector3 targetPoint; // for the visual trail

        // Bitshift to ignore layer 8 (usually the player) so we don't shoot ourselves
        // Ensure Player is on the "Player" layer!
        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            Debug.Log("Shot hit: " + hit.transform.name);
            targetPoint = hit.point;

            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                // Cast the float 'damage' to an int if your interface uses int
                target.TakeDamage((int)damage);
                Debug.Log("Ranged hit dealt " + damage + " damage!");
            }

            // If the target has a Rigidbody, push it
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * 100f);
            }
        }
        else
        {
            // If we missed, shoot into the distance
            targetPoint = ray.GetPoint(range);
        }

        // 3. VISUAL TRAIL
        StartCoroutine(ShowTrail(targetPoint));
    }

    IEnumerator RecoilPush()
    {
        float duration = 0.1f;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            // move backwards relative to player facing
            _controller.Move(-transform.forward * recoilForce * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator ShowTrail(Vector3 hitPoint)
    {
        bulletTrail.enabled = true;

        // If gunTip is missing, use the player's position plus a little height
        Vector3 startPoint = gunTip != null ? gunTip.position : transform.position + Vector3.up * 1.2f;

        bulletTrail.SetPosition(0, startPoint);
        bulletTrail.SetPosition(1, hitPoint);
        yield return new WaitForSeconds(0.1f);
        bulletTrail.enabled = false;
    }
}
