using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float movePower;
    [SerializeField] private float jumpPower;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] private GameObject playerAttack;

    private Rigidbody2D rb;
    private Vector2 inputDir;
    private SpriteRenderer render;
    private bool isGround;
    private Collider2D col;

    private int jumpCount = 0;

    InputAction playerAction;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
 
    }
    private void Start()
    {
        playerAttack.SetActive(false);
    }
    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        GroundCheck();
    }
    private void Move()
    {
        
        if (inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
        else if ( inputDir.x > 0 && rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        jumpCount++;
    }

    private void OnMove(InputValue value)
    {
        inputDir = value.Get<Vector2>();
        if (inputDir.x > 0)
            render.flipX = false;
        else if (inputDir.x < 0)
            render.flipX = true;
        
    }
  
    private void OnJump(InputValue value)
    {
        if (isGround && jumpCount == 0)
        {
            Jump();
        }
        else if (jumpCount == 1)
        {
            Jump();
            jumpCount = 0;
        }
    }

    private void OnAttack(InputValue value)
    {
        playerAttack.SetActive(true);
        StartCoroutine(AttackFalse());
    }
    //Attack collider가 1초만 지속되도록 하기 위함
    IEnumerator AttackFalse()
    {
        yield return new WaitForSeconds(1f);
        playerAttack.SetActive(false);
    }

    private void GroundCheck()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer);
        
        
        if (hit.collider != null)
        {
            //Debug.Log(hit.collider.gameObject.name);
            isGround = true;

            //Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red);
           
        }
        else
        {
            isGround = false;

            
            //Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.red);
        }
    }
}
