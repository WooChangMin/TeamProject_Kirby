using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Yaban : MonoBehaviour                      // �߹� : �����ؼ� �밢�� �Ʒ��� Ȱ��� ĳ����
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
                Debug.Log("����");
                IdleUpdate();
                break;
            //case State.Jumping:
            //    Debug.Log("����");
            //    JumpingUpdate();
                break;
            case State.Attack:
                Debug.Log("����");
                AttackUpdate();
                break;
        }
    }
    float idleTime = 0;
    private void IdleUpdate()
    {
        if (Vector2.Distance(player.position, transform.position) < detectRange) // �÷��̾ ��������� ��
        {
            curState = State.Jumping; // ������

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
