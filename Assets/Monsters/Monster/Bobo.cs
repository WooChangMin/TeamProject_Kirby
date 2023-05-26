using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bobo : MonoBehaviour
{
    // 레이어에서 Ground를 설정해줘야 무한회전을 안함
    // 충돌하면 돌아가기 위해 bobo 앞에 있는 장애물 tag에 Wall을 설정해줘야함
    // 피가 0이면 bobo는 사라짐
    [SerializeField] private float moveSpeed;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundMask;
    [SerializeField] private float boboHp;

    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Move();
        BoboDie();
       
    }
    private void FixedUpdate()
    {
        if (!IsGroundExist()) // 땅이 없으면 반대방향으로
        {
            Debug.Log("벽");
            Turn();
        }
    }
    private void BoboDie()
    {
        if (boboHp <= 0f)
        {
            Debug.Log("Bobo가 죽었다.");
            Destroy(gameObject);
        }
    }
    public void Turn()
    {
        transform.Rotate(Vector3.up, 180);
    }

    private void Move()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeed, rigidbody2d.velocity.y);
    }
    private bool IsGroundExist()
    {
        Debug.DrawRay(groundCheckPoint.position, Vector2.left, Color.red);
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, 1l, groundMask);
    }
    private void OnCollisionEnter2D(Collision2D collision)                                              // collision을 이용해 Wall 태그가 붙은 물체와 충돌시 뒤로돌기
    {
    if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("충돌");
            Turn();
        }
    }
}
