using UnityEngine;

public class BallReturnOnMiss : MonoBehaviour
{
    public string outOfPlayTag = "OutOfPlay";
    public float delay = 0.6f;

    void OnCollisionEnter(Collision c)
    {
        if (c.collider.CompareTag(outOfPlayTag))
            GetComponent<BallHomeReturn>()?.ReturnAfter(delay);
    }
}
