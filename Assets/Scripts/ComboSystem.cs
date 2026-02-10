using UnityEngine;
using UnityEngine.UI;

public class ComboSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    public float comboResetTime = 1.0f;
    public int comboCount = 0;

    [Header("Visual Feedback (The Traffic Lights")]
    public Image attack1Indicator; // green
    public Image attack2Indicator; // yellow
    public Image attack3Indicator; // red

    // reference to the inputs
    private StarterAssets.StarterAssetsInputs _input;
    private float _lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
        ResetCombo();
    }

    // Update is called once per frame
    void Update()
    {
        // 1. check if the combo has timed out (player waited too long)
        if (Time.time - _lastAttackTime > comboResetTime && comboCount > 0)
        {
            ResetCombo();
        }

        // 2. listen for attack input
        if (_input.attack)
        {
            PerformAttack();
            _input.attack = false; // reset the flag instantly so we don't spam
        }
    }

    void PerformAttack()
    {
        // update the timer
        _lastAttackTime = Time.time;

        // increment the combo step
        comboCount++;

        // handle the logic based on which step we are on
        if (comboCount == 1)
        {
            Debug.Log("Combo Step 1");
            UpdateUI(1);
        }
        else if (comboCount == 2)
        {
            Debug.Log("Combo Step 2");
            UpdateUI(2);
        }
        else if (comboCount == 3) // Changed from >= to ==
        {
            Debug.Log("Combo Step 3: FINISHER!");
            UpdateUI(3);
        }
        else
        {
            // This prevents 4, 5, 6+ clicks from doing anything 
            // until the combo naturally resets.
            Debug.Log("Finisher recovery... wait for reset.");
        }
    }
    void UpdateUI(int step)
    {
        // turn everything off first for safety
        attack1Indicator.color = new Color(0, 1, 0, 0.2f); // dim green
        attack2Indicator.color = new Color(0, 0.92f, 0.016f, 0.2f); // dim yellow
        attack3Indicator.color = new Color(1, 0, 0, 0.2f); // dim red

        // turn on the specific light
        if (step == 1) attack1Indicator.color = Color.green;
        if (step == 2) attack2Indicator.color = Color.yellow;
        if (step == 3) attack3Indicator.color = Color.red;

    }

    void ResetCombo()
    {
        comboCount = 0;
        Debug.Log("Combo Reset");
        UpdateUI(0); // dim all lights
    }
}
