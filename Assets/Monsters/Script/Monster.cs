using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform[] patrolPoints;

    private Animator animator;
    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    public void Move()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeed, rigidbody2d.velocity.y);
    }
}
