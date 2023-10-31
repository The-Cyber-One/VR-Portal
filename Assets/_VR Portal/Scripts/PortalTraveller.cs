using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTraveller : MonoBehaviour
{
    [HideInInspector] public Vector3 PreviousOffsetFromPortal;
    [SerializeField] protected Transform Parent;
    [SerializeField] protected Rigidbody RB;
    private Collider _collider;


    private void Awake()
    {
        if (Parent == null)
            Parent = transform;

        _collider = GetComponent<Collider>();
        if (RB == null)
        {
            TryGetComponent(out RB);
        }
    }

    public virtual void Teleport(Portal inPortal, Portal outPortal)
    {
        Matrix4x4 portalMatrix = outPortal.transform.localToWorldMatrix * inPortal.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        Matrix4x4 teleportMatrix = portalMatrix * transform.worldToLocalMatrix * Parent.localToWorldMatrix;

        Parent.SetPositionAndRotation(teleportMatrix.GetPosition(), teleportMatrix.rotation);

        inPortal.TravellerExitedPortal(this);
        outPortal.TravellerEnterdPortal(this);

        if (RB != null)
        {
            Vector3 localVelocity = inPortal.transform.InverseTransformDirection(RB.velocity);
            Vector3 newVelocity = outPortal.transform.TransformDirection(localVelocity);
            RB.velocity = newVelocity;
        }
    }

    public virtual void SetPortalCollision(bool ignoreCollision, Collider wallCollider)
    {
        Physics.IgnoreCollision(_collider, wallCollider, ignoreCollision);
    }
}
