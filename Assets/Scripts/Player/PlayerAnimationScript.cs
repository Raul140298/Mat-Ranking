using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private RenderingScript compRendering;

    public void OnMovement(InputAction.CallbackContext value)
    {
        float movementInput = value.ReadValue<Vector2>().magnitude;

        if (movementInput != 0f)
        {
            compRendering.PlayAnimation(eAnimation.Walk);
        }
        else
        {
            compRendering.PlayAnimation(eAnimation.Idle);
        }
    }
}
