// Dart.cs  (UPDATED for XRI)  :contentReference[oaicite:0]{index=0}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dart : MonoBehaviour
{
    [Header("Throw Tuning")]
    public float baseThrowPower = 3.5f;   // scales XRI-provided velocity
    [Range(12f, 40f)] public float weightGrams = 22f;

    Rigidbody rb;
    TrailRenderer trail;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        ApplyWeight();
        // When sitting in the hand rack, keep kinematic so it doesn't fall
        rb.isKinematic = true;
        rb.useGravity = false;
        if (trail) { trail.emitting = false; trail.Clear(); }
    }

    public void ApplyWeight()
    {
        rb.mass = Mathf.Max(0.01f, weightGrams / 1000f);
        rb.linearDamping = Mathf.Lerp(0.02f, 0.08f, Mathf.InverseLerp(12f, 40f, weightGrams));
        rb.angularDamping = 0.05f;
    }

    // called by XRThrowableOnActivate when user grabs (Select entered)
    public void OnGrabbed()
    {
        rb.isKinematic = false;
        rb.useGravity = false; // darts fly mostly ballistic by velocity; keep gravity off or very low if you prefer
        if (trail) { trail.Clear(); trail.emitting = false; }
    }

    // called by XRThrowableOnActivate when it applies velocity and releases
    public void OnReleased(Vector3 baseVel)
    {
        Vector3 throwVel = baseVel * baseThrowPower;
        rb.linearVelocity = throwVel;

        if (throwVel.sqrMagnitude > 0.01f)
            rb.MoveRotation(Quaternion.LookRotation(throwVel.normalized, Vector3.up));

        if (trail) { trail.emitting = true; Invoke(nameof(StopTrail), 2.0f); }
    }

    // called by XRThrowableOnActivate when dropped without throwing
    public void OnReleasedNoThrow()
    {
        // keep as-is; optionally snap back to left-hand slot if you want
        if (trail) trail.emitting = false;
    }

    void StopTrail() { if (trail) trail.emitting = false; }
}
