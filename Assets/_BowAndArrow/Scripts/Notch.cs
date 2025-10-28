using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Notch : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
{
    [SerializeField, Range(0, 1)] private float releaseThreshold = 0.25f;
    [SerializeField] private GameObject arrowPrefab;//new

    public Bow Bow { get; private set; }
    public PullMeasurer PullMeasurer { get; private set; }
    private Rigidbody arrowRb;

    public bool CanRelease => PullMeasurer.PullAmount > releaseThreshold;

    protected override void Awake()
    {
        base.Awake();
        Bow = GetComponentInParent<Bow>();
        PullMeasurer = GetComponentInChildren<PullMeasurer>();
        if (PullMeasurer != null) //new
            PullMeasurer.notch = this;

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PullMeasurer.selectEntered.AddListener(StartPull);//new
        PullMeasurer.selectExited.AddListener(ReleaseArrow);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PullMeasurer.selectEntered.RemoveListener(StartPull);//new
        PullMeasurer.selectExited.RemoveListener(ReleaseArrow);
    }
    private void StartPull(SelectEnterEventArgs args) //new
    {
        if (!hasSelection) // Only spawn arrow if one isn't already notched
        {
            Arrow arrow = CreateArrow(PullMeasurer.transform);
            interactionManager.SelectEnter((IXRSelectInteractor)this, (IXRSelectInteractable)arrow);
        }
    }
    private Arrow CreateArrow(Transform orientation)
    {
        GameObject arrowObj = Instantiate(arrowPrefab, orientation.position, orientation.rotation);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        return arrow;
    }

    public void ReleaseArrow(SelectExitEventArgs args)
    {
        //if (hasSelection)
        //    interactionManager.SelectExit(this, firstInteractableSelected);
        if (!hasSelection) return;

        if (CanRelease)
        {
            // Fully release arrow (shoot)
            interactionManager.SelectExit(
                (IXRSelectInteractor)this,
                (IXRSelectInteractable)firstInteractableSelected
            );
        }
        else
        {
            // Keep the arrow socketed — do nothing
            PullMeasurer.ResetPull(); // Reset pull value manually
        }
    }
    public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractor(updatePhase);

        if (Bow.isSelected)
            UpdateAttach();
    }

    public void UpdateAttach()
    {
        // Move attach when bow is pulled, this updates the renderer as well
        attachTransform.position = PullMeasurer.PullPosition;
    }

    public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        // We check for the hover here too, since it factors in the recycle time of the socket
        // We also check that notch is ready, which is set once the bow is picked up
        return QuickSelect(interactable) && CanHover(interactable) && interactable is Arrow && Bow.isSelected;
    }

    private bool QuickSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        // This lets the Notch automatically grab the arrow
        return !hasSelection || IsSelecting(interactable);
    }

    private bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
    {
        if (interactable is UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable hoverInteractable)
            return CanHover(hoverInteractable);

        return false;
    }
}
