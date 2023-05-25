using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    enum State { normal, Fire, Broom, Boomerang , Size}

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float movePower;
    [SerializeField] private float levitationPower;
    [SerializeField] private LayerMask groundMask;

    
    

    private Vector2 inputDir;

    private new SpriteRenderer renderer;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGround;
    private bool isLeviting;
    private State curState;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        GroundChecker();
    }
   private void Move()
    {
        if (inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower);
        else if (inputDir.x > 0 && rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower);
    }

    private void OnMove(InputValue value)
    {
        inputDir = value.Get<Vector2>();
        anim.SetFloat("MoveDirX", Mathf.Abs(inputDir.x));
        if (inputDir.x > 0)
            renderer.flipX = false;
        else if (inputDir.x < 0)
            renderer.flipX = true;

    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);        
    }
    private void OnJump(InputValue value)
    {
        if(value.isPressed && isGround)
            Jump();

    }

    private void GroundChecker()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down , 1.5f, groundMask);
        if(hit.collider != null)
        {
            isGround = true;
            anim.SetBool("IsGround", true);
        }
        else
        {
            isGround = false;
            anim.SetBool("IsGround", false);
        }
    }

    



    /*    private void fly()
        {
            if (!isGround && isLeviting)
                rb.velocity = Vector2.up * levitationPower;
        }
    */
    //ispressed

}