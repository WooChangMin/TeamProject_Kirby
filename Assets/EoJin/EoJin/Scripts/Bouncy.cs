using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    [SerializeField] private int angle = 60;
    [SerializeField] private float force;
    private Rigidbody2D rb;
    private float goDown;
    [SerializeField] float lessFoundation;
    private float time;
    private bool leftOrRight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {

    }
    private void Update()
    {
        time += Time.deltaTime;

        if (time > 2f)
        {
            time = 0;
            leftOrRight = !leftOrRight;

            if (leftOrRight)
                JumpRight();
            else
                JumpLeft();
        }


    }
    private void JumpRight()
    {
        float foundation = Mathf.Cos(angle * Mathf.PI / 180);
        float height = Mathf.Sin(angle * Mathf.PI / 180);

        rb.AddRelativeForce(new Vector2(foundation - lessFoundation, height) * force, ForceMode2D.Impulse);
    }
    private void JumpLeft()
    {
        float foundation = Mathf.Cos(angle * Mathf.PI / 180);
        float height = Mathf.Sin(angle * Mathf.PI / 180);

        rb.AddRelativeForce(new Vector2( - (foundation - lessFoundation), height) * force, ForceMode2D.Impulse);
    }

}