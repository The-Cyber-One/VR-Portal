using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PortalTraveller : MonoBehaviour
{
    [HideInInspector] public Vector3 PreviousOffsetFromPortal;
    [SerializeField] protected Transform Parent;

    private void Awake()
    {
        if (Parent == null)
            Parent = transform;
    }

    public virtual void Teleport(Portal inPortal, Portal outPortal)
    {
        Matrix4x4 portalMatrix = outPortal.transform.localToWorldMatrix * inPortal.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        Matrix4x4 teleportMatrix = portalMatrix * transform.worldToLocalMatrix * Parent.localToWorldMatrix;

        Parent.SetPositionAndRotation(teleportMatrix.GetPosition(), teleportMatrix.rotation);

        inPortal.TravellerExitedPortal(this);
        outPortal.TravellerEnterdPortal(this);
    }
}
