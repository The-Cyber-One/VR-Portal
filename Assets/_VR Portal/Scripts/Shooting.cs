using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform leftHand, rightHand;
    [SerializeField] InputAction inputActionLeft, inputActionRight;
    [SerializeField] Portal portalLeft, portalRight;
    [SerializeField] float wallDistance = 0.05f;

    [Header("Physic cast")]
    [SerializeField] float castRadius = 0.1f, maxDistance = 100;
    [SerializeField] LayerMask nonPortalLayer;

    private void OnEnable()
    {
        inputActionLeft.performed += OnLeftButtonPressed;
        inputActionRight.performed += OnRightButtonPressed;
        inputActionLeft.Enable();
        inputActionRight.Enable();
    }

    private void OnDisable()
    {
        inputActionLeft.performed -= OnLeftButtonPressed;
        inputActionRight.performed -= OnRightButtonPressed;
        inputActionLeft.Disable();
        inputActionRight.Disable();
    }

    private void OnLeftButtonPressed(InputAction.CallbackContext obj) => ShootPortal(leftHand, portalLeft);
    private void OnRightButtonPressed(InputAction.CallbackContext obj) => ShootPortal(rightHand, portalRight);

    private void ShootPortal(Transform pointer, Portal portal)
    {
        if (Physics.SphereCast(pointer.position, castRadius, pointer.forward, out RaycastHit hit, maxDistance, nonPortalLayer))
        {
            Debug.Log("hit at: " + hit.point);
            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 1f);
            _previousHit = hit.point;
            portal.transform.position = hit.point + hit.normal * wallDistance;
        }
    }

    Vector3 _previousHit;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_previousHit, 0.1f);
    }
}
