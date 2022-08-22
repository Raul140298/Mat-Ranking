using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public FromLevelSO fromLevel;
    public BoxCollider2D dialogueArea;
    //public ParticleSystem dustPS;

    private Vector2 _movementInput;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = _movementInput * speed;

        if (SceneManager.GetActiveScene().buildIndex == 1)
		{           
            if (rb.velocity.x > 0) // cambio de direccion
            {
                //var x = dustPS.velocityOverLifetime;
                //x.x = -0.8f;
                //if (_movementInput.x < 0f) dustPS.Play();
                if(dialogueArea) dialogueArea.offset = new Vector2(1.5f, 0.25f);
            }
            if (rb.velocity.x < 0) // cambio de direccion
            {
                //var x = dustPS.velocityOverLifetime;
                //x.x = 0.8f;
                //if (_movementInput.x > 0f) dustPS.Play();
                if(dialogueArea) dialogueArea.offset = new Vector2(-1.5f, 0.25f);
            }
        }
   //     if (SceneManager.GetActiveScene().buildIndex == 2)
   //     {
   //         if(_movementInput.x > 0.75) //go to right
			//{
   //             transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, speed * Time.deltaTime);
			//}
   //         if (_movementInput.x < -0.75) //go to left
   //         {
   //             transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, speed * Time.deltaTime);
   //         }
   //         if (_movementInput.y > 0.75) //go to top
   //         {
   //             transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, speed * Time.deltaTime);
   //         }
   //         if (_movementInput.y < -0.75) //go to down
   //         {
   //             transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, speed * Time.deltaTime);
   //         }
   //     }
    }

    public void OnMovement(InputAction.CallbackContext value)
	{
        _movementInput = value.ReadValue<Vector2>();
	}
}
