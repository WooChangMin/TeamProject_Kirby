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
    //BattleRange�� AttackRange�� �÷��̾�� ���� ��ŭ ����(������) ����
    //�ٵ� �ʹ� ������ collider���� �ε����� ���̻� ������ ������ ��
    [SerializeField] private float battleRange;
    [SerializeField] private Player player;

    //������ �� �� �ִٰ�(SetActive(true) ���ϰ� �ϱ� ���� gameObject
    //�� ������Ʈ�� �ݶ��̴� �ȿ� ��� �÷��̾ ������ ����
    [SerializeField] private GameObject AttackRange;

    //������ �������� �޴� ������ ���ӿ�����Ʈ(�÷��̾�ų� �θ޶��̰ų�)
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

        Debug.Log(targetFlyPoint);

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
        transform.position = Vector2.SmoothDamp(gameObject.transform.position, playerTransform.position, ref vel, 1f);

        //�÷��̾ �־����� ��
        if (Vector2.Distance(transform.position, playerTransform.position) > detectRange)
            curState = State.Idle;

        //�÷��̾ ���ݹ��� �ȿ� ���
        if (Vector2.Distance(transform.position, playerTransform.position) < battleRange)
            curState = State.Battle;
    }

    private float BattleWaitTime;
    [SerializeField] private int life = 3;
    private void BattleUpdate()
    {
        //Battle -> attack, trace
        BattleWaitTime += Time.deltaTime;
        //Time.deltaTime -> 1�ʴ� �ɸ� ������ �������ִ� �� 30fs -> 1/30
        //Update���� BattleUpdate�� ����Ǹ鼭 1�ʰ� ��� �帣�� �� ���� ����
        //�׳� 0.02f(������ ���ŵǴ� �ֱ��ε�) ���ؼ� �ð� �����ϴ� �ź��� ������ ������ �Ǵϱ� Time.deltaTime ���ٴ� ��

        //onCollisionEnter���¿��� 3�� ���� Attack�� �����ϰ�
        if (BattleWaitTime > 3)
        {
            BattleWaitTime = 0;
            Attack();
        }

        //�÷��̾ battle range ����� trace ���·�
        //�ٵ� battle range -> trace �� trace -> battle range�� ��ġ�Ƿ�
        //battle range���� 1��ŭ �� �־����� trace ���·� ���� ��
        if (Vector2.Distance(transform.position, playerTransform.position) > battleRange + 1)
            curState = State.Trace;

        //battle Range ���� �÷��̾�� ���� ���� �ٵ� �ʹ� �����̰��� �о�ϱ� ������� �Ÿ� ����
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
    //Attack collider�� 1�ʸ� ���ӵǵ��� �ϱ� ����
    IEnumerator AttackFalse()
    {
        yield return new WaitForSeconds(1f);
        AttackRange.SetActive(false);
    }

    //�÷��̾��� ���ݿ� ������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision Entered");
        if (collision.collider == playerAttackCol)
        {
            Debug.Log("aaa");
            StartCoroutine(DamagedForOnce());
        }
    }
    //�÷��̾� ������ 1�ʵ��� �ȴٸ� 1�ʵ��� 1���� ������ ���̰� �ϱ� ���� �ڷ�ƾ
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
