
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private GameObject destroyEffect;

    private Rigidbody2D rigidbody2d;
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        rigidbody2d.velocity = -transform.right * ProjectileSpeed;
    }
}
