
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] Transform projectile;
    private Rigidbody2D rigidbody2d;
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * ProjectileSpeed;
    }
    private void Update()
    {
        transform.right = GetComponent<Rigidbody2D>().velocity;
        Destroy(gameObject, 6f);
    }
}
