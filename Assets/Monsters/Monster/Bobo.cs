using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bobo : MonoBehaviour
{
    // ���̾�� Ground�� ��������� ����ȸ���� ����
    // �浹�ϸ� ���ư��� ���� bobo �տ� �ִ� ��ֹ� tag�� Wall�� �����������
    // �ǰ� 0�̸� bobo�� �����
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
        if (!IsGroundExist()) // ���� ������ �ݴ��������
        {
            Debug.Log("��");
            Turn();
        }
    }
    private void BoboDie()
    {
        if (boboHp <= 0f)
        {
            Debug.Log("Bobo�� �׾���.");
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
    private void OnCollisionEnter2D(Collision2D collision)                                              // collision�� �̿��� Wall �±װ� ���� ��ü�� �浹�� �ڷε���
    {
    if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("�浹");
            Turn();
        }
    }
}
