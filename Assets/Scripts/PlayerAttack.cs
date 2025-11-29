using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Setup")]
    public float swingDuration = 0.3f;
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Swing Settings")]
    [Tooltip("The total arc the sword swings across (e.g., 90 degrees).")]
    public float swingArc = 90f;
    [Tooltip("The starting angle offset for the swing (e.g., to swing forward, use -45).")]
    public float swingStartOffset = -45f;
    [Tooltip("Distance the sword thrusts out at the peak of the swing.")]
    public float thrustDistance = 0.5f;

    private GameObject currentWeapon;
    private DamageOnContact damageScript;
    private bool isAttacking = false;
    private Quaternion originalRotation;
    private Vector3 originalLocalPosition;

    public void SetWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        damageScript = weapon.GetComponent<DamageOnContact>();

        // Store the initial rotation and position set by the PlayerInteraction script
        originalRotation = currentWeapon.transform.localRotation;
        originalLocalPosition = currentWeapon.transform.localPosition;

        if (damageScript == null)
        {
            Debug.LogError("The equipped weapon is missing the DamageOnContact script!");
        }
    }

    public void ClearWeapon()
    {
        currentWeapon = null;
        damageScript = null;
        isAttacking = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && currentWeapon != null && !isAttacking)
        {
            StartCoroutine(PerformAttackSwing());
        }
    }

    private IEnumerator PerformAttackSwing()
    {
        isAttacking = true;

        // 1. ACTIVATE DAMAGE
        if (damageScript != null)
        {
            damageScript.StartAttack();
        }

        // --- COMBO ANIMATION LOGIC (Rotation & Thrust) ---

        // Calculate Rotation Swing Variables
        Quaternion currentHandRotation = currentWeapon.transform.localRotation;
        Quaternion startRotation = currentHandRotation * Quaternion.Euler(0, 0, swingStartOffset);
        Quaternion endRotation = currentHandRotation * Quaternion.Euler(0, 0, swingStartOffset + swingArc);

        // Calculate Thrust Position Variables
        // The thrust is applied along the sword's forward axis (transform.right) relative to the hand.
        Vector3 maxThrustPosition = originalLocalPosition + currentWeapon.transform.right * thrustDistance;

        float timer = 0f;

        // Loop runs for the entire swing duration
        while (timer < swingDuration)
        {
            float t = timer / swingDuration;

            // A. Rotation (Smooth Swing Arc)
            currentWeapon.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            // B. Position (Thrust Out and Back In)

            // We want the thrust to peak at t=0.5 (mid-swing)
            // Function f(t) = 4 * t * (1 - t) creates a smooth arc peaking at 1.0 when t=0.5
            float thrustCurve = 4f * t * (1f - t);

            // Interpolate the sword position from its original position to the max thrust position
            currentWeapon.transform.localPosition = Vector3.Lerp(originalLocalPosition, maxThrustPosition, thrustCurve);

            timer += Time.deltaTime;
            yield return null;
        }

        // 2. DEACTIVATE DAMAGE
        if (damageScript != null)
        {
            damageScript.EndAttack();
        }

        // 3. RESET
        // Force reset the position and rotation to the perfect idle state to prevent drift
        currentWeapon.transform.localRotation = originalRotation;
        currentWeapon.transform.localPosition = originalLocalPosition;

        isAttacking = false;
    }
}