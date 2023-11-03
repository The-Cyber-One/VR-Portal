using UnityEngine;

public class PlayerPortalTraveller : PortalTraveller
{
    [SerializeField] Rigidbody handLeft, handRight;
    [SerializeField] Transform xrOrigin;

    public override void Teleport(Portal inPortal, Portal outPortal)
    {
        Vector3 up = xrOrigin.up;

        base.Teleport(inPortal, outPortal);

        Vector3 forward = Vector3.ProjectOnPlane(xrOrigin.forward, up);
        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }
        var rot = Quaternion.LookRotation(forward, up);

        Vector3 originalPos = transform.position;
        xrOrigin.rotation = rot;
        xrOrigin.position += originalPos - transform.position;

        handLeft.velocity = RB.velocity;
        handRight.velocity = RB.velocity;
    }
}