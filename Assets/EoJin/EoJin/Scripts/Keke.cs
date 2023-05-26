using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Keke : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public enum State { Idle, Trace, Fly, Battle}
    private State curState;

    [SerializeField] float moveSpeed;
    [SerializeField] public Transform[] FlyPoints;
    [SerializeField] private float detectRange;
    //BattleRange는 AttackRange가 플레이어에게 닿을 만큼 적게(가깝게) 설정
    //근데 너무 가까우면 collider끼리 부딪혀서 더이상 가깝게 못가는 듯
    [SerializeField] private float battleRange;
    [SerializeField] private Player player;

    //공격을 할 수 있다가(SetActive(true) 못하게 하기 위해 gameObject
    //이 오브젝트의 콜라이더 안에 들면 플레이어가 데미지 받음
    [SerializeField] private GameObject AttackRange;

    //맞으면 데미지를 받는 무언가의 게임오브젝트(플레이어거나 부메랑이거나)
    [SerializeField] private Collider2D playerAttackCol;

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
                text.text = "Idle";
                IdleUpdate();
                break;
            case State.Fly:
                text.text = "Fly";
                FlyUpdate();
                break;
            case State.Trace:
                text.text = "Trace";
                TraceUpdate();
                break;
            case State.Battle:
                text.text = "Battle";
                BattleUpdate();
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

        Debug.Log(targetFlyPoint);

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
        transform.position = Vector2.SmoothDamp(gameObject.transform.position, playerTransform.position, ref vel, 1f);

        //플레이어가 멀어졌을 때
        if (Vector2.Distance(transform.position, playerTransform.position) > detectRange)
            curState = State.Idle;

        //플레이어가 공격범위 안에 들면
        if (Vector2.Distance(transform.position, playerTransform.position) < battleRange)
            curState = State.Battle;
    }

    private float BattleWaitTime;
    [SerializeField] private int life = 3;
    private void BattleUpdate()
    {
        //Battle -> attack, trace
        BattleWaitTime += Time.deltaTime;
        //Time.deltaTime -> 1초당 걸린 프레임 보정해주는 값 30fs -> 1/30
        //Update에서 BattleUpdate가 실행되면서 1초가 계속 흐르고 그 값을 쓴다
        //그냥 0.02f(프레임 갱신되는 주기인듯) 더해서 시간 측정하는 거보다 프레임 보정이 되니까 Time.deltaTime 쓴다는 듯

        //onCollisionEnter상태에서 3초 마다 Attack이 가능하게
        if (BattleWaitTime > 3)
        {
            BattleWaitTime = 0;
            Attack();
        }

        //플레이어가 battle range 벗어나면 trace 상태로
        //근데 battle range -> trace 랑 trace -> battle range가 겹치므로
        //battle range에서 1만큼 더 멀어지면 trace 상태로 가게 함
        if (Vector2.Distance(transform.position, playerTransform.position) > battleRange + 1)
            curState = State.Trace;

        //battle Range 들어서도 플레이어에게 비비기 위함 근데 너무 가까이가면 밀어내니까 어느정도 거리 더함
        Vector2 closeToPlayer = new Vector2(player.transform.position.x + 1f, player.transform.position.y + 1f);
        transform.position = Vector2.SmoothDamp(gameObject.transform.position, closeToPlayer, ref vel, 1f);
    }

    private void Attack()
    {
        anim.SetTrigger("IsAttack");
        //Attack -> battle
        AttackRange.SetActive(true);
        StartCoroutine(AttackFalse());
    }
    //Attack collider가 1초만 지속되도록 하기 위함
    IEnumerator AttackFalse()
    {
        yield return new WaitForSeconds(1f);
        AttackRange.SetActive(false);
    }

    //플레이어의 공격에 맞으면
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision Entered");
        if (collision.collider == playerAttackCol)
        {
            Debug.Log("aaa");
            StartCoroutine(DamagedForOnce());
        }
    }
    //플레이어 공격이 1초동안 된다면 1초동안 1번만 생명이 깎이게 하기 위해 코루틴
    IEnumerator DamagedForOnce()
    {
        damaged();
        yield return new WaitForSeconds(1f);
    }

    private void damaged()
    {
        //damaged -> die
        life -= 1;
        if (life == 0)
            Die();
    }
    private void Die()
    {
        gameObject.SetActive(false);
    }


}
