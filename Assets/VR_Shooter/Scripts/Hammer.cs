using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { Throw, Return, Idle }
public class Hammer : MonoBehaviour
{
    State state;
    Rigidbody rb;
    public float hammerSpeed;
    public Transform playerHand;
    public float returnDistance = 1.0f; // Define a distance threshold for return

    private void Start()
    {
        state = State.Idle;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Throw:
                if (rb.linearVelocity.magnitude > 0)  // Prevent division by zero
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * hammerSpeed;
                }
                break;

            case State.Return:
                Vector3 direction = (playerHand.position - transform.position);
                if (direction.sqrMagnitude > returnDistance * returnDistance)
                {
                    rb.linearVelocity = direction.normalized * hammerSpeed;
                }
                else
                {
                    state = State.Idle;
                    rb.linearVelocity = Vector3.zero;
                }
                break;

            case State.Idle:
                break;
        }
    }

    public void ThrowHammer()
    {
        state = State.Throw;
    }

    public void ReturnHammer()
    {
        state = State.Return;
    }

    public void IdleHammer()
    {
        state = State.Idle;
        rb.linearVelocity = Vector3.zero;
    }
}
