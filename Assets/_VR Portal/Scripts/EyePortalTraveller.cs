using UnityEngine;

public class EyePortalTraveller : PortalTraveller
{
    [SerializeField] Transform xrOrigin;
    [SerializeField] EyePortalTraveller otherEye;
    [SerializeField] Camera cam;

    private bool _teleported = false;

    public override void Teleport(Portal inPortal, Portal outPortal)
    {
        // If moving second eye move entire xr origin
        if (otherEye._teleported)
        {
            var tmpParent = Parent;
            Parent = xrOrigin;
            base.Teleport(inPortal, outPortal);
            Parent = tmpParent;
            otherEye.Parent.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            otherEye._teleported = false;
        }
        else
        {
            base.Teleport(inPortal, outPortal);
            _teleported = true;
        }

        outPortal.ProtectScreenFromClipping();
    }
}
