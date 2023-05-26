using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Keke : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public enum State { Idle, Trace, Fly, Battle, AttackWait, Attack, Die}
    private State curState;

    [SerializeField] float moveSpeed;
    [SerializeField] public Transform[] FlyPoints;
    [SerializeField] private float detectRange;
    //BattleRange�� AttackRange�� �÷��̾�� ���� ��ŭ ����(������) ����
    //�ٵ� �ʹ� ������ collider���� �ε����� ���̻� ������ ������ ��
    [SerializeField] private float battleRange;
    [SerializeField] private Player player;
    private Collider2D playerCol;

    //������ �� �� �ִٰ�(SetActive(true) ���ϰ� �ϱ� ���� gameObject
    //�� ������Ʈ�� �ݶ��̴� �ȿ� ��� �÷��̾ ������ ����
    [SerializeField] private GameObject AttackRange;

    private Transform playerTransform;
    private Collider2D kekeCol;
    private Animator anim;
    private Rigidbody2D rb;

    private bool isDie;
    Vector2 vel = Vector3.zero;

    private int targetFlyPoint = 0;

    private float waitTime;

    private void Awake()
    {
        playerTransform = player.GetComponent<Player>().transform;
        kekeCol = gameObject.GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCol = player.GetComponent<Collider2D>();
    }
    private void Start()
    {
        AttackRange.SetActive(false);
        rb.gravityScale = 0f;
        curState = State.Idle;
    }
    private void Update()
    {
        switch (curState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Fly:
                FlyUpdate();
                break;
            case State.Trace:
                TraceUpdate();
                break;
            case State.Battle:
                BattleUpdate();
                break;
            case State.AttackWait:
                AttackWaitUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
            case State.Die:
                DieUpdate();
                break;
        }
    }

    /*
     * ���� ��ȯ
     * idle -> fly
     * fly -> idle, trace
     * trace -> battle, idle
     * Battle -> attack, trace
     * Attack -> battle
     * 
     * damaged(collision Enter) -> die
     * die
     */

    private void IdleUpdate()
    {
        //idle -> fly

        //��� �׳� ����(������1) -> ����(������2)�� ���� ��ȯ�� �մ� �߰� �ܰ� ������ �̿�
        //Fly(����) ���� -> ���̵�(���⼭ ���� ������ ���) -> Fly ����
        if (targetFlyPoint == FlyPoints.Length - 1)
            targetFlyPoint = 0;
        else
            targetFlyPoint++;


        curState = State.Fly;
    }
    private void FlyUpdate()
    {
        //fly -> idle, trace

        //���� ��ġ���� ������ �������� �̵�
        transform.position = Vector2.SmoothDamp(transform.position, FlyPoints[targetFlyPoint].position, ref vel, 1f);

        //���� ���� ��ġ���� ���ٰ� ���� ��ġ ~ �������� �Ÿ�(����)�� 0.02���� �۾�����(�������)
        if (Vector2.Distance(transform.position, FlyPoints[targetFlyPoint].position) < 0.02f)
            //�ٽ� idle�� ���� fly ������ ����
            curState = State.Idle;

        //�÷��̾� ��ġ ~ ���� ��ġ�Ÿ��� �������� �ȿ� ��� curState�� Trace�� ����
        if (Vector2.Distance(transform.position, playerTransform.position) < detectRange)
            curState = State.Trace;
    }
    private void TraceUpdate()
    {
        //trace -> battle, return

        //�Ѿư���
        Vector2 closeToPlayer = new Vector2(player.transform.position.x + 0.5f, player.transform.position.y + 0.5f);
        transform.position = Vector2.SmoothDamp(gameObject.transform.position, closeToPlayer, ref vel, 1f);
        //�÷��̾ �־����� ��
        if (Vector2.Distance(transform.position, playerTransform.position) > detectRange)
            curState = State.Idle;

        //�÷��̾ ���ݹ��� �ȿ� ���
        if (Vector2.Distance(transform.position, playerTransform.position) < battleRange)
        {
            curState = State.Battle;
        }
    }


    private void BattleUpdate()
    {

        //Battle -> attackWait, trace

        rb.velocity = new Vector2(0, 0);

        //�÷��̾�� ������ �ٰ�����
        //Vector2 closeToPlayer = new Vector2(player.transform.position.x + 2f, player.transform.position.y + 2f);
        //transform.position = Vector2.SmoothDamp(gameObject.transform.position, closeToPlayer, ref vel, 1f);

        //�÷��̾ battle range ����� trace ���·�
        //�ٵ� battle range -> trace �� trace -> battle range�� ��ġ�Ƿ�
        //battle range���� 1��ŭ �� �־����� trace ���·� ���� ��
        if (Vector2.Distance(transform.position, playerTransform.position) > battleRange + 1)
            curState = State.Trace;

        curState = State.AttackWait;
    }

    private float attackWaitTime = 0f;

    private void AttackWaitUpdate()
    {
        //attackWait -> attack, Trace

        attackWaitTime += Time.deltaTime;
        //Debug.Log(attackWaitTime);

        if (attackWaitTime > 3f)
        {
            Debug.Log("aa");
            attackWaitTime = 0;
            curState = State.Attack;
        }

        if (Vector2.Distance(transform.position, playerTransform.position) > battleRange + 1)
            curState = State.Trace;
    }

    private float attackTime = 0f;
    private void AttackUpdate()
    {
        //Attack -> AttackWait
       
        AttackRange.SetActive(true);
        anim.SetBool("IsAttack", true);

        attackTime += Time.deltaTime;

        if (attackTime > 1.4f)
        {
            anim.SetBool("IsAttack", false);
            AttackRange.SetActive(false);
            curState = State.AttackWait;
            attackTime = 0f;
        }

    }

    //�÷��̾� �ݸ����� ������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("playerCollision Entered");
        curState = State.Die;
    }
    
    private void DieUpdate()
    {
        anim.Play("kekeFalling");
        Invoke("SetActiveFalse", 0.4f);
    }

    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

}
