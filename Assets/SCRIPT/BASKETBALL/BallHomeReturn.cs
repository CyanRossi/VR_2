using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallHomeReturn : MonoBehaviour
{
    public Transform homeSocket;
    public float respawnDelay = 0.35f;

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    public void PutAtHome()
    {
        if (!homeSocket) return;
        rb.isKinematic = true;
        transform.SetParent(homeSocket, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnGrabbed()
    {
        rb.isKinematic = false;
        transform.SetParent(null);
    }

    public void OnReleased(Vector3 releaseVel)
    {
        rb.linearVelocity = releaseVel;
        GetComponent<AimAssist>()?.BeginAssist();
    }

    public void ReturnAfter(float delay) => Invoke(nameof(PutAtHome), delay);
}
