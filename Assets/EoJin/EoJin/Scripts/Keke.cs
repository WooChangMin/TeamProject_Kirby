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
    //BattleRange는 AttackRange가 플레이어에게 닿을 만큼 적게(가깝게) 설정
    //근데 너무 가까우면 collider끼리 부딪혀서 더이상 가깝게 못가는 듯
    [SerializeField] private float battleRange;
    [SerializeField] private Player player;
    private Collider2D playerCol;

    //공격을 할 수 있다가(SetActive(true) 못하게 하기 위해 gameObject
    //이 오브젝트의 콜라이더 안에 들면 플레이어가 데미지 받음
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
     * 상태 전환
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

        //얘는 그냥 순찰(목적지1) -> 순찰(목적지2)의 상태 전환을 잇는 중간 단계 정도로 이용
        //Fly(순찰) 상태 -> 아이들(여기서 다음 목적지 계산) -> Fly 상태
        if (targetFlyPoint == FlyPoints.Length - 1)
            targetFlyPoint = 0;
        else
            targetFlyPoint++;


        curState = State.Fly;
    }
    private void FlyUpdate()
    {
        //fly -> idle, trace

        //현재 위치에서 정해진 목적지로 이동
        transform.position = Vector2.SmoothDamp(transform.position, FlyPoints[targetFlyPoint].position, ref vel, 1f);

        //만약 현재 위치에서 가다가 현재 위치 ~ 목적지의 거리(길이)가 0.02보다 작아지면(가까워짐)
        if (Vector2.Distance(transform.position, FlyPoints[targetFlyPoint].position) < 0.02f)
            //다시 idle로 가서 fly 목적지 변경
            curState = State.Idle;

        //플레이어 위치 ~ 현재 위치거리가 감지범위 안에 들면 curState를 Trace로 변경
        if (Vector2.Distance(transform.position, playerTransform.position) < detectRange)
            curState = State.Trace;
    }
    private void TraceUpdate()
    {
        //trace -> battle, return

        //쫓아가기
        Vector2 closeToPlayer = new Vector2(player.transform.position.x + 0.5f, player.transform.position.y + 0.5f);
        transform.position = Vector2.SmoothDamp(gameObject.transform.position, closeToPlayer, ref vel, 1f);
        //플레이어가 멀어졌을 때
        if (Vector2.Distance(transform.position, playerTransform.position) > detectRange)
            curState = State.Idle;

        //플레이어가 공격범위 안에 들면
        if (Vector2.Distance(transform.position, playerTransform.position) < battleRange)
        {
            curState = State.Battle;
        }
    }


    private void BattleUpdate()
    {

        //Battle -> attackWait, trace

        rb.velocity = new Vector2(0, 0);

        //플레이어에게 가까이 다가가기
        //Vector2 closeToPlayer = new Vector2(player.transform.position.x + 2f, player.transform.position.y + 2f);
        //transform.position = Vector2.SmoothDamp(gameObject.transform.position, closeToPlayer, ref vel, 1f);

        //플레이어가 battle range 벗어나면 trace 상태로
        //근데 battle range -> trace 랑 trace -> battle range가 겹치므로
        //battle range에서 1만큼 더 멀어지면 trace 상태로 가게 함
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

    //플레이어 콜리젼에 맞으면
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
