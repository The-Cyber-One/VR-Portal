using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] InputAction inputAction;
    [SerializeField] Portal portal;
    [SerializeField] bool isLeftPortal;
    [SerializeField] PortalProjectile portalProjectile;
    [SerializeField] LayerMask wallLayerMask, nonPortalMask;
    [SerializeField] float maxWallDistance = 100;

    private PortalProjectile _portalProjectileInstance;

    private void OnEnable()
    {
        inputAction.performed += OnButtonPressed;
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.performed -= OnButtonPressed;
        inputAction.Disable();
    }

    private void OnButtonPressed(InputAction.CallbackContext obj)
    {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxWallDistance, nonPortalMask))
            return; // Don't start shooting if nothing will be hit.

        if (_portalProjectileInstance != null)
        {
            Destroy(_portalProjectileInstance.gameObject);
        }

        _portalProjectileInstance = Instantiate(portalProjectile, transform.position, transform.rotation);
        _portalProjectileInstance.StartMove(hit.point);
        _portalProjectileInstance.OnHit += () =>
        {
            if (((1 << hit.collider.gameObject.layer) & wallLayerMask) != 0 && ShootPortal(hit))
                return;

            // Indicate mis fire
            Debug.Log($"Can't place portal on {hit.transform.gameObject.name}");
        };
    }

    private bool ShootPortal(RaycastHit hit)
    {
        Vector3 portalPosition = hit.point + hit.normal * portal.PortalScreen.localScale.z;
        Vector3 portalNormal = isLeftPortal ? hit.normal : -hit.normal;
        // Use the rotation of the controller for up if portal is on ceiling or floor
        Vector3 upDirection = Mathf.Abs(Vector3.Dot(portalNormal, Vector3.up)) > 0.9f ? transform.up : Vector3.up;
        Quaternion portalRotation = Quaternion.LookRotation(portalNormal, upDirection);

        FixOverhangs(ref portalPosition, portalRotation);
        FixIntersections(ref portalPosition, portalRotation);
        Debug.DrawLine(portalPosition, transform.position, Color.yellow, 5);

        if (CheckOverlap())
        {
            portal.MovePortal(hit.collider, portalPosition, portalRotation);
            return true;
        }

        return false;
    }

    private void FixOverhangs(ref Vector3 portalPosition, Quaternion portalRotation)
    {
        float portalScreenXOffset = portal.PortalScreen.localScale.x / 2;
        float portalScreenYOffset = portal.PortalScreen.localScale.y / 2;
        float maxOffset = Mathf.Max(portalScreenXOffset, portalScreenYOffset) + 0.05f;

        var testPoints = new[]
        {
            portalRotation * new Vector3(-portalScreenXOffset, 0, -portal.PortalScreen.localScale.z - 0.01f),
            portalRotation * new Vector3(portalScreenXOffset, 0, -portal.PortalScreen.localScale.z - 0.01f),
            portalRotation * new Vector3(0, -portalScreenYOffset, -portal.PortalScreen.localScale.z - 0.01f),
            portalRotation * new Vector3(0, portalScreenYOffset, -portal.PortalScreen.localScale.z - 0.01f),
        };

        var testDirections = new[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,
        };

        for (int i = 0; i < testPoints.Length; i++)
        {
            Debug.DrawLine(portalPosition + testPoints[i], transform.position, Color.white, 5);
            if (Physics.CheckSphere(portalPosition + testPoints[i], 0.05f, wallLayerMask))
                continue; // Not break as was shown in the video

            if (Physics.Raycast(portalPosition + testPoints[i], testDirections[i], out RaycastHit hit, maxOffset))
            {
                Vector3 offset = hit.point - (portalPosition + testPoints[i]);
                Debug.DrawRay(portalPosition + testPoints[i], offset, Color.cyan, 5);
                portalPosition += offset;
            }
        }
    }

    private void FixIntersections(ref Vector3 portalPosition, Quaternion portalRotation)
    {
        float portalScreenXOffset = portal.PortalScreen.localScale.x / 2;
        float portalScreenYOffset = portal.PortalScreen.localScale.y / 2;

        var testDirections = new[]
        {
            portalRotation * Vector3.right,
            portalRotation * Vector3.left,
            portalRotation * Vector3.up,
            portalRotation * Vector3.down,
        };

        var testDistances = new[]
        {
            portalScreenXOffset,
            portalScreenXOffset,
            portalScreenYOffset,
            portalScreenYOffset,
        };

        for (int i = 0; i < testDirections.Length; i++)
        {
            if (Physics.Raycast(portalPosition, testDirections[i], out RaycastHit hit, testDistances[i]))
            {
                Vector3 offset = portalPosition - hit.point;
                Vector3 displacement = -testDirections[i] * (testDistances[i] - offset.magnitude);
                portalPosition += displacement;
            }
        }
    }

    private bool CheckOverlap()
    {
        return true;
    }
}
