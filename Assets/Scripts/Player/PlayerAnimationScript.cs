using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationScript : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void OnMovement(InputAction.CallbackContext value)
    {
        float movementInput = value.ReadValue<Vector2>().magnitude;

        if (movementInput > 0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
