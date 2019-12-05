using ObjectTub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private IEnumerator despawnAfterSomeTime(float someTime)
    {
        yield return new WaitForSeconds(someTime);
        ObjectPool.PutObjectBackInTub(gameObject);
    }

    public override void InitializeForUse()
    {
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StartCoroutine(despawnAfterSomeTime(3f));
    }

    public override void PutAway()
    {
        trailRenderer.Clear();
    }
}
