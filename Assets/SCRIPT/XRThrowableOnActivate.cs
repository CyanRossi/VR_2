// XRThrowableOnActivate.cs  (XRI 3.2.1-compatible)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class XRThrowableOnActivate : XRGrabInteractable
{
    [Header("Throw Tuning")]
    [Tooltip("Multiply interactor velocity on release")]
    public float releaseSpeedScale = 1.1f;   // 1.0–1.5
    [Tooltip("Adds upward arc proportional to speed")]
    public float upwardBias = 0.15f;         // 0–0.25
    [Tooltip("Clamp final speed (m/s)")]
    public float maxSpeed = 14f;
    [Tooltip("Optional: scale angular velocity on release")]
    public float angularVelScale = 1.0f;

    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        // We will set velocity manually when Activate is pressed.
        throwOnDetach = false;
        trackPosition = true;
        trackRotation = true;
        retainTransformParent = false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (rb) rb.isKinematic = false;

        // Notify held object (e.g., Dart) if it listens.
        SendMessage("OnGrabbed", SendMessageOptions.DontRequireReceiver);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Dropped without an Activate throw.
        SendMessage("OnReleasedNoThrow", SendMessageOptions.DontRequireReceiver);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        // Interactor that pressed Activate (Trigger/Pinch)
        var selectInteractor = args.interactorObject as IXRSelectInteractor;
        if (selectInteractor == null)
            return;

        PerformThrowAndRelease(selectInteractor);
    }

    private void PerformThrowAndRelease(IXRSelectInteractor interactor)
    {
        if (interactor == null || rb == null)
            return;

        // --- Get interactor transform & velocity from XRVelocityTracker ---
        // NOTE: IXRSelectInteractor is implemented by a Component (MonoBehaviour), so cast to Component to get Transform.
        var interactorComp = interactor as Component;
        Vector3 vel = Vector3.zero;
        Vector3 angVel = Vector3.zero;

        if (interactorComp != null)
        {
            // Prefer an XRVelocityTracker on the interactor object (Direct/Hand Interactor).
            var tracker = interactorComp.GetComponent<XRVelocityTracker>();
            if (tracker != null)
            {
                vel = tracker.velocity * releaseSpeedScale;
                angVel = tracker.angularVelocity * angularVelScale;
            }
            else
            {
                // Fallback: approximate linear velocity from attach transform delta (last frame).
                // This is a light fallback; adding XRVelocityTracker to your interactor is recommended.
                var attach = interactor.GetAttachTransform(this);
                // If there’s no tracker, we can’t robustly compute angular velocity here; leave it zero.
                // You can optionally implement your own attach-delta velocity cache if needed.
                vel = rb.linearVelocity; // use current as minimal fallback
            }
        }

        // Add a bit of upward arc
        vel += Vector3.up * (upwardBias * Mathf.Max(0f, vel.magnitude));

        // Clamp speed
        if (vel.magnitude > maxSpeed)
            vel = vel.normalized * maxSpeed;

        // Detach from interactor and apply velocities
        interactionManager?.SelectExit(interactor, this);

        rb.linearVelocity = vel;
        rb.angularVelocity = angVel;

        // Notify listeners (e.g., Dart trail/power, aim assist)
        SendMessage("OnReleased", vel, SendMessageOptions.DontRequireReceiver);
        GetComponent<AimAssist>()?.BeginAssist();

        // Optional: event hook you use elsewhere
        var d = GetComponent<Dart>();
        if (d) GameEvents.OnDartThrown?.Invoke(d);
    }
}
