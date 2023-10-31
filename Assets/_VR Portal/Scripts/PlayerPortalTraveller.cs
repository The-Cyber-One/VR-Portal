using UnityEngine;

public class PlayerPortalTraveller : PortalTraveller
{
    [SerializeField] Rigidbody handLeft, handRight;
    [SerializeField] Transform xrOrigin;

    public override void Teleport(Portal inPortal, Portal outPortal)
    {
        base.Teleport(inPortal, outPortal);
        xrOrigin.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(xrOrigin.forward, Vector3.up), Vector3.up);
        handLeft.velocity = RB.velocity;
        handRight.velocity = RB.velocity;
    }
}