using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    enum State {Normal, Die, Full, Fire, Broom, Boomerang,  Size}


    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float movePower;
    [SerializeField] private float levitationPower;
    [SerializeField] private LayerMask groundMask;

    GameObject gameobj;
    private float hp = 10;

    private Vector2 inputDir;


    private new SpriteRenderer renderer;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGround;
    private bool isLeviting;
    private bool isSlide;
    private State curState;
    private bool isPressed;

   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPressing)
            Eat();
        else
            Debug.Log("not pressing");

        if(!isSlide)
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        
    }

    //충돌시 뒤로 팅겨나게
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "monster")
        {
            //Attack - 3.22
            if(!isSlide)
                OnDamaged(collision.transform.position); //Damaged - 3.22
        }
    }
    public void OnDamaged(Vector2 TargetPos)
    {//-3.22
     //Change Layer
        hp --;
        if (hp == 0)
        {
            Debug.Log("죽었습니다.");
            curState = State.Die;
        }
        else
        {
            gameObject.layer = 20;
            //Readctio Force 3.22

            renderer.color = new Color(1, 1, 1, 0.4f);
            int dirc = transform.position.x - TargetPos.x > 0 ? 1 : -1;
            rb.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

            //anim.SetTrigger("Die"); //- 3.25
            Invoke("OffDamaged", 3);
        }
    }
    void OffDamaged()   
    {
        gameObject.layer = 3;
        renderer.color =new Color(1, 1, 1, 1);
    }

    private void Eat()
    {
        
        Debug.DrawRay(transform.position, Vector2.right * 5f, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 5f, groundMask);
    }

    private void Slide()
    {
        if (inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            rb.velocity = Vector2.right * inputDir.x * slideSpeed;
        else if (inputDir.x > 0)
            rb.velocity = Vector2.right * inputDir.x * slideSpeed;
    }

    private void OnSlide(InputValue value)
    {
        if (isGround)
        {
            Slide();
            anim.SetBool("IsSlide", true);
            isSlide = true;

            Invoke("ChangeState", 0.35f);
        }
    }

    private void ChangeState()
    {
        anim.SetBool("IsSlide", false);
        isSlide = false;
    }

    private bool isPressing;
    public void OnEat(InputValue value)
    {
        isPressing = value.isPressed;

        if (isPressing)
            Debug.Log("KeyDown");
        else
            Debug.Log("KeyUp");
    }
}