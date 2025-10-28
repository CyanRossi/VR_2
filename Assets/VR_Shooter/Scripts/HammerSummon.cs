using UnityEngine;
using UnityEngine.InputSystem;
public class HammerSummon : MonoBehaviour
{
    public InputActionReference rightPrimaryButton;
    public Hammer hammer;

    void Start()
    {
        rightPrimaryButton.action.performed += PrimaryButtonPressed;
    }

    void OnDestroy()
    {
        rightPrimaryButton.action.performed -= PrimaryButtonPressed; // Unsubscribe to avoid memory leaks
    }

    void PrimaryButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("PrimaryButton is Pressed");
        hammer.ReturnHammer();
    }

}
