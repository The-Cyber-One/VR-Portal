using UnityEngine;

public class PortalDisabler : PortalTraveller
{
    [SerializeField] Collider[] colliders;

    public override void Teleport(Portal inPortal, Portal outPortal)
    {
        // Leave empty so that portal will only disable the collision
    }

    public override void SetPortalCollision(bool active, Collider wallCollider)
    {
        base.SetPortalCollision(active, wallCollider);

        foreach (var collider in colliders)
        {
            Physics.IgnoreCollision(collider, wallCollider, active);
        }
    }
}