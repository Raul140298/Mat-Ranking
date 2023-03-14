using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private FromLevelSO fromLevel;
    [SerializeField] private BoxCollider2D dialogueArea;

    private Vector2 movementInput;

    void FixedUpdate()
    {
        rb.velocity = movementInput.normalized * speed;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (rb.velocity.x > 0) // cambio de direccion
            {
                if (dialogueArea) dialogueArea.offset = new Vector2(1.5f, 0.25f);
            }
            if (rb.velocity.x < 0) // cambio de direccion
            {
                if (dialogueArea) dialogueArea.offset = new Vector2(-1.5f, 0.25f);
            }
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();
    }
}
