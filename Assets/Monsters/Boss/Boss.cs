using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // �ӵ�
    [SerializeField] private float startTime; // ���۽ð�
    [SerializeField] private float loopTime;  // �ݺ��ð�

    [SerializeField] private float moveSpeedUp;
    [SerializeField] private float startUpTime;
    [SerializeField] private float loopUpTime;

    [SerializeField] protected float bossHp;

    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {

    }


    private void Update()
    {

        if (bossHp <= 4)
        {
            InvokeRepeating("Move", startTime, loopTime);
            InvokeRepeating("SpeedMove", startUpTime, loopUpTime);
        }
    }
    public void Move()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeed, rigidbody2d.velocity.y);
    }
    public void SpeedMove()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeedUp, rigidbody2d.velocity.y);
    }
    private void TrueDamage(float damage)
    {
        //ap = bossHp - damage;
    }


}
