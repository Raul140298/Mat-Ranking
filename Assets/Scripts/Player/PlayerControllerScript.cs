using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerScript : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public FromLevelSO fromLevel;
    public BoxCollider2D dialogueArea;

    private Vector2 movementInput;

    void FixedUpdate()
    {
        rb.velocity = movementInput * speed;

        if (SceneManager.GetActiveScene().buildIndex == 1)
		{           
            if (rb.velocity.x > 0) // cambio de direccion
            {
                if(dialogueArea) dialogueArea.offset = new Vector2(1.5f, 0.25f);
            }
            if (rb.velocity.x < 0) // cambio de direccion
            {
                if(dialogueArea) dialogueArea.offset = new Vector2(-1.5f, 0.25f);
            }
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
	{
        movementInput = value.ReadValue<Vector2>();
	}
}
