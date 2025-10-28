// XRVelocityTracker.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Lightweight velocity tracker for any interactor (hand or controller).
/// Adds accurate linear & angular velocity sampling every frame.
/// </summary>
[DisallowMultipleComponent]
public class XRVelocityTracker : MonoBehaviour
{
    [Tooltip("How many frames to average for smoothing.")]
    [Range(1, 10)] public int smoothingFrames = 5;

    public Vector3 velocity { get; private set; }
    public Vector3 angularVelocity { get; private set; }

    Vector3 lastPos;
    Quaternion lastRot;
    Vector3[] velSamples;
    int index;
    bool hasPrev;

    void Awake()
    {
        velSamples = new Vector3[smoothingFrames];
    }

    void FixedUpdate()
    {
        var pos = transform.position;
        var rot = transform.rotation;

        if (hasPrev)
        {
            // linear velocity (m/s)
            var v = (pos - lastPos) / Time.fixedDeltaTime;
            velSamples[index] = v;
            index = (index + 1) % velSamples.Length;
            velocity = AverageVel();

            // angular velocity (rad/s)
            var deltaRot = rot * Quaternion.Inverse(lastRot);
            deltaRot.ToAngleAxis(out float angleDeg, out Vector3 axis);
            if (angleDeg > 180f) angleDeg -= 360f;
            angularVelocity = axis * (angleDeg * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }

        lastPos = pos;
        lastRot = rot;
        hasPrev = true;
    }

    Vector3 AverageVel()
    {
        Vector3 sum = Vector3.zero;
        int count = Mathf.Min(velSamples.Length, index);
        for (int i = 0; i < count; i++) sum += velSamples[i];
        return count > 0 ? sum / count : Vector3.zero;
    }
}
