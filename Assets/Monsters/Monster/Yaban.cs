using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Yaban : MonoBehaviour                      // 야반 : 점프해서 대각선 아래로 활쏘는 캐릭터
{
    [SerializeField] private float jumpPower;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    private Rigidbody2D rigidbody2d;
    private State curState;
    private Transform player;

    private void Jump()
    {
        rigidbody2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
    public enum State { Idle, Jumping, Attack}
    private void Update()
    {
        switch (curState)
        {
            case State.Idle:
                Debug.Log("가만");
                IdleUpdate();
                break;
            //case State.Jumping:
            //    Debug.Log("점프");
            //    JumpingUpdate();
                break;
            case State.Attack:
                Debug.Log("공격");
                AttackUpdate();
                break;
        }
    }
    float idleTime = 0;
    private void IdleUpdate()
    {
        if (Vector2.Distance(player.position, transform.position) < detectRange) // 플레이어가 가까워졌을 때
        {
            curState = State.Jumping; // 점프함

        }
        else
        {

        }
    }   

    private void AttackUpdate()
    {

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
