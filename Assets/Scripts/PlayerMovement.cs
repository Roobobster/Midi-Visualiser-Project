using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{


    [SerializeField]
    private float baseSpeed;

    [SerializeField]
    private float sprintSpeedMultiplier;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float jumpHorizontalThrust;


    private float currentSpeedModifier;

    [SerializeField]
    private Transform baiTransform;
    [SerializeField]
    private Transform maoTransform;

    [SerializeField]
    private Slider baiEnergyBar;
    [SerializeField]
    private Slider maoEnergyBar;

    private float speedMultiplier;

    private bool canJump;
    private bool hasStopped;

    // Start is called before the first frame update
    void Start()
    {
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckVerticalMovement();
        CheckHorizontalMovement();
    }


    private void CheckHorizontalMovement() 
    {

        currentSpeedModifier = 0;
        Vector3 moveDirection = Vector3.zero;

        //Checks which direction to move
        if (Input.GetKey("d"))
        {
            moveDirection = Vector3.right;           
        }
        else if (Input.GetKey("a"))
        {
            moveDirection = Vector3.left;        
        }

        RotateSprites(moveDirection);


        //Checks to see if sprinting or not
        speedMultiplier = 1;
        if (Input.GetKey("left shift"))
        {
            speedMultiplier = sprintSpeedMultiplier;
        }


        //Applies speed modifiers and passes to animator to display correct animation based on speed
        MoveSprites(moveDirection, speedMultiplier);



    }

    private void RotateSprites(Vector3 direction) 
    {
        if (direction != Vector3.zero) 
        {
            currentSpeedModifier = 1;
            baiTransform.localScale = new Vector3( direction.x, 1, 1);
            maoTransform.localScale = new Vector3( direction.x, 1, 1);
        }
        
    }

    private void MoveSprites(Vector3 moveDirection, float speedMultiplier) 
    {
        if (canJump) 
        {
            currentSpeedModifier *= speedMultiplier;
            SetAnimatorSpeedParameters(currentSpeedModifier);
            Rigidbody2D playerRigidBody2d = GetComponent<Rigidbody2D>();
            playerRigidBody2d.velocity = new Vector2(moveDirection.x * baseSpeed * currentSpeedModifier, playerRigidBody2d.velocity.y);
        }
        
    }


    private void SetAnimatorSpeedParameters(float currentSpeed)
    {

        baiTransform.GetComponent<Animator>().SetFloat("Move Speed", currentSpeed);
        maoTransform.GetComponent<Animator>().SetFloat("Move Speed", currentSpeed);

    }

    private void CheckVerticalMovement()
    {
        //Updates if jumping for the animator
        maoTransform.GetComponent<Animator>().SetBool("Jumping", !canJump);
        baiTransform.GetComponent<Animator>().SetBool("Jumping", !canJump);

        Rigidbody2D playerRigidBody2D = GetComponent<Rigidbody2D>();

        if (Input.GetKey("space") && canJump)
        {
            
            
            Vector2 resetVelocity = new Vector2(playerRigidBody2D.velocity.x + (baiTransform.localScale.x * jumpHorizontalThrust), 0);
            playerRigidBody2D.velocity = resetVelocity;
            playerRigidBody2D.AddForce(Vector3.up * jumpHeight);
           

            canJump = false;


            baiTransform.GetComponent<Animator>().SetTrigger("JustJumped");
            maoTransform.GetComponent<Animator>().SetTrigger("JustJumped");
        }

        if (playerRigidBody2D.velocity.magnitude < 0.1f)
        {
            if (!hasStopped) 
            {
                maoTransform.GetComponent<Animator>().SetTrigger("JustStopped");
                baiTransform.GetComponent<Animator>().SetTrigger("JustStopped");
                hasStopped = true;
            }
            
        }
        else 
        {
            hasStopped = false;
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            canJump = false;
        }
        else if (collision.CompareTag("Power Up"))
        {
            baiEnergyBar.value += 50;
            maoEnergyBar.value += 100;
            Destroy(collision.gameObject);
        
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            canJump = true;
        }
    }
}
