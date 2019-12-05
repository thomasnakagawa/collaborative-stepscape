using ObjectTub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PoolableObject
{
    [SerializeField] private float MaxHP = 100f;

    private float currentHP;

    private Player player;
    private float speed = 0.5f;

    public override void InitializeForUse()
    {
        transform.localScale = Vector3.one;
        currentHP = MaxHP;
        player = FindObjectOfType<Player>();
    }

    public override void PutAway()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player.transform);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponentInParent<Bullet>() != null)
        {
            float damage = collision.relativeVelocity.magnitude;
            currentHP -= damage;

            transform.localScale = new Vector3(1f, Mathf.Max(currentHP / MaxHP, 0.1f), 1f);

            if (currentHP < 0f)
            {
                ObjectPool.PutObjectBackInTub(gameObject);
            }
        }
    }
}
