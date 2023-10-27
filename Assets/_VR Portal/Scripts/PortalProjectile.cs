using System;
using UnityEngine;

public class PortalProjectile : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] Rigidbody rb;
    [SerializeField] float maxWallDistance = 1;

    public event Action<RaycastHit> OnHit;

    private void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxWallDistance))
        {
            OnHit.Invoke(hit);
            Destroy(gameObject);
        }
    }
}
