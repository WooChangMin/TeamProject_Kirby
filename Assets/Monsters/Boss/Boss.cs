using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Search;


public class Boss : MonoBehaviour
{
    [Header("Boss Idle")] 
    [SerializeField] private float moveSpeed; // ¼Óµµ      1
    [SerializeField] private float startTime; // ½ÃÀÛ½Ã°£  0.5
    [SerializeField] private float loopTime;  // ¹Ýº¹½Ã°£  1
    [Header("Boss Anger")]
    [SerializeField] private float moveSpeedUp; // 3
    [SerializeField] private float startUpTime; // 1
    [SerializeField] private float loopUpTime;  // 1.5
    [Header("Boss Fire")]
    [SerializeField] private GameObject projectilePoint;
    [SerializeField] private GameObject projectilePrefab;
    // [SerializeField] private float projectileStartTime;
    // [SerializeField] private float projectileTime;
    [SerializeField] private float attckTime;   // 1
    [Header("Boss Anger Fire")]
    [SerializeField] private GameObject AngerPoint;
    [SerializeField] private GameObject AngerPrefab;
    // [SerializeField] private float AngerTime;
    [SerializeField] private float Angerattck;  // 1.5
    [Header("BossHp")]
    [SerializeField] private float bossHp;      // 8
    private Animator animator;


    private Rigidbody2D rigidbody2d;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        Invoke("IdleUpadte", 1f);
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        //InvokeRepeating("Fire", projectileStartTime, projectileTime);
        // if (bossHp > 5f)
        // {
        //     InvokeRepeating("Fire", projectileStartTime, projectileTime);
        // }

    }
    private enum State { Idle, Anger, Die, Sleep}
    private State state = State.Idle;


    private void Update()
    {
        switch (state)
        {
            case State.Sleep:
                SleepUpdate();
                break;
            case State.Idle:
                IdleUpadte();
                break;
            case State.Anger:
                AngerUpdate();
                break;
            case State.Die:
                DieUpdate();
                break;
        }
        //if (bossHp <= 4f)
        //{
        //    InvokeRepeating("Move", startTime, loopTime);
        //    InvokeRepeating("SpeedMove", startUpTime, loopUpTime);
        //}
        //else if (bossHp <= 0) 
        //{
        //    Destroy(gameObject);
        //    //rigidbody2d.velocity = Vector3.zero;
        //}
    }
    private void SleepUpdate()
    {
        if (bossHp <= 4f)
        {
            animator.SetBool("bossHp", true);
            Invoke("AngerUpdate", 1.2f);
        }
    }
    float lastAttackTime = 0;
    private void IdleUpadte()
    {
        
        if (lastAttackTime > attckTime)
        {
            Fire();
            lastAttackTime = 0;
            Debug.Log("Çª½¹");
        }
        lastAttackTime += Time.deltaTime;

        if (bossHp <= 4f)
        {
            state = State.Sleep;
        }
    }
    float AngerAttackTime = 0;
    private void AngerUpdate()
    {
        InvokeRepeating("Move", startTime, loopTime);
        InvokeRepeating("SpeedMove", startUpTime, loopUpTime);
        if (AngerAttackTime > Angerattck)
        {
            AngerFire();
            AngerAttackTime = 0;
            Debug.Log("È­³µÇª½¹");
        }
        AngerAttackTime += Time.deltaTime;

        if (bossHp <= 0f)
        {
            state = State.Die;
        }
    }
    private void DieUpdate()
    {
        CancelInvoke();
        animator.SetBool("Die", true);
        Debug.Log("À¸¾Ç"); 
    }
    public void Move()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeed, rigidbody2d.velocity.y);
    }
    public void SpeedMove()
    {
        rigidbody2d.velocity = new Vector2(transform.right.x * -moveSpeedUp, rigidbody2d.velocity.y);
    }
    private void Fire()
    {
        Instantiate(projectilePrefab, projectilePoint.transform.position, projectilePoint.transform.rotation);
    }
    private void AngerFire()
    {
        Instantiate(AngerPrefab, AngerPoint.transform.position, AngerPoint.transform.rotation);
    }
}
