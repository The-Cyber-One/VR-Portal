using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] InputAction inputAction;
    [SerializeField] Portal portal;
    [SerializeField] bool isLeftPortal;

    [Header("Physic cast")]
    [SerializeField] float maxDistance = 100;
    [SerializeField] LayerMask wallLayerMask;

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
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, wallLayerMask))
        {
            if (!ShootPortal(hit))
            {
                // Indicate mis fire
            }
        }
    }

    private bool ShootPortal(RaycastHit hit)
    {
        Vector3 portalPosition = hit.point + hit.normal * portal.PortalScreen.localScale.z;
        Quaternion portalRotation = Quaternion.LookRotation(isLeftPortal ? hit.normal : -hit.normal);

        FixOverhangs(ref portalPosition);
        FixIntersections(ref portalPosition, ref portalRotation);

        if (CheckOverlap())
        {
            portal.MovePortal(hit.collider, portalPosition, portalRotation);
            return true;
        }

        return false;
    }

    private void FixOverhangs(ref Vector3 portalPosition)
    {
        float portalScreenXOffset = portal.PortalScreen.localScale.x / 2;
        float portalScreenYOffset = portal.PortalScreen.localScale.y / 2;
        float maxOffset = Mathf.Max(portalScreenXOffset, portalScreenYOffset) + 0.05f;

        var testPoints = new[]
        {
            new Vector3(-portalScreenXOffset, 0, 0),
            new Vector3(portalScreenXOffset, 0, 0),
            new Vector3(0, -portalScreenYOffset, 0),
            new Vector3(0, portalScreenYOffset, 0),
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
            if (Physics.CheckSphere(portalPosition + testPoints[i], 0.05f, wallLayerMask))
                continue; // Not break as was shown in the video

            else if (Physics.Raycast(testPoints[i], testDirections[i], out RaycastHit hit, maxOffset))
            {
                Vector3 offset = hit.point - portalPosition;
                portalPosition += offset;
            }
        }
    }

    private void FixIntersections(ref Vector3 portalPosition, ref Quaternion portalRotation)
    {
        float portalScreenXOffset = portal.PortalScreen.localScale.x / 2;
        float portalScreenYOffset = portal.PortalScreen.localScale.y / 2;

        var testDirections = new[]
        {
            Vector3.right * portalScreenXOffset,
            Vector3.left * portalScreenXOffset,
            Vector3.up * portalScreenYOffset,
            Vector3.down * portalScreenYOffset,
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
            if (Physics.Raycast(portalPosition, testDirections[i], out RaycastHit hit, testDistances[i], wallLayerMask))
            {
                Vector3 offset = hit.point - portalPosition;
            }
        }
    }

    private bool CheckOverlap()
    {
        return true;
    }
}
