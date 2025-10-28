using UnityEngine;
using UnityEngine.Events;

public class TriggerEventInvoker : MonoBehaviour
{
    [Header("Trigger Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }
}
