using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AimAssist : MonoBehaviour
{
    [Header("Target to bias towards (bull or rim center)")]
    public Transform target;
    [Tooltip("Seconds after release where assist is applied")]
    public float activeTime = 0.2f;
    [Tooltip("Only assist if angle to target is below this (deg)")]
    public float maxAdjustAngle = 6f;
    [Tooltip("Acceleration toward target direction")]
    public float accel = 25f;
    [Tooltip("Limit extra speed gained by assist")]
    public float maxSpeedGain = 2f;

    Rigidbody rb;
    float tLeft;
    float initSpeed;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    public void BeginAssist()
    {
        if (!rb || !target) return;
        initSpeed = rb.linearVelocity.magnitude;
        tLeft = activeTime;
    }

    void FixedUpdate()
    {
        if (tLeft <= 0f || !rb || !target) return;
        tLeft -= Time.fixedDeltaTime;

        Vector3 v = rb.linearVelocity;
        if (v.sqrMagnitude < 1e-4f) return;

        Vector3 to = (target.position - rb.worldCenterOfMass).normalized;
        float ang = Vector3.Angle(v, to);
        if (ang > maxAdjustAngle) return;

        Vector3 steer = Vector3.ProjectOnPlane(to, v.normalized);
        rb.AddForce(steer * accel, ForceMode.Acceleration);

        float speed = rb.linearVelocity.magnitude;
        if (speed > initSpeed * maxSpeedGain)
            rb.linearVelocity = rb.linearVelocity.normalized * (initSpeed * maxSpeedGain);
    }
}
