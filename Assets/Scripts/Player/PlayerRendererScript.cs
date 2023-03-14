using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRendererScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 movementInput = value.ReadValue<Vector2>();

        if (movementInput.x > 0.01f && PlayerIsLookingLeft())
        {
            spriteRenderer.flipX = false;
        }
        else if (movementInput.x < -0.01f && !PlayerIsLookingLeft())
        {
            spriteRenderer.flipX = true;
        }
    }

    public bool PlayerIsLookingLeft()
    {
        return spriteRenderer.flipX;
    }

    public SpriteRenderer SpriteRenderer => spriteRenderer;
}
