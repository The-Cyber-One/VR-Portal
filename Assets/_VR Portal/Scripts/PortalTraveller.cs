using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTraveller : MonoBehaviour
{
    [HideInInspector] public Vector3 PreviousOffsetFromPortal;
    [SerializeField] protected Transform Parent;
    public Collider Collider { get; private set; }

    private Rigidbody rb;

    private void Awake()
    {
        if (Parent == null)
            Parent = transform;

        Collider = GetComponent<Collider>();
        TryGetComponent(out rb);
    }

    public virtual void Teleport(Portal inPortal, Portal outPortal)
    {
        Matrix4x4 portalMatrix = outPortal.transform.localToWorldMatrix * inPortal.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        Matrix4x4 teleportMatrix = portalMatrix * transform.worldToLocalMatrix * Parent.localToWorldMatrix;

        Parent.SetPositionAndRotation(teleportMatrix.GetPosition(), teleportMatrix.rotation);

        inPortal.TravellerExitedPortal(this);
        outPortal.TravellerEnterdPortal(this);

        if (rb != null)
        {
            Vector3 localVelocity = inPortal.transform.InverseTransformDirection(rb.velocity);
            rb.velocity = outPortal.transform.TransformDirection(localVelocity);
        }
    }
}
