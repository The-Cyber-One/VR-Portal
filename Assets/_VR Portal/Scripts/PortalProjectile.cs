using System;
using System.Collections;
using UnityEngine;

public class PortalProjectile : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] Rigidbody rb;
    [SerializeField] float lifeTime = 10;

    public event Action OnHit;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    public void StartMove(Vector3 endPoint, float bodyVelocity)
    {
        rb.velocity = transform.forward * (speed + bodyVelocity);
        StartCoroutine(Move(endPoint));
    }

    private IEnumerator Move(Vector3 endPoint)
    {
        yield return new WaitUntil(() => Vector3.Dot(endPoint - transform.position, transform.forward) < 0);
        OnHit?.Invoke();
        Destroy(gameObject);
    }
}
