using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform leftHand, rightHand;
    [SerializeField] InputAction inputActionLeft, inputActionRight;
    [SerializeField] Portal portalLeft, portalRight;
    [SerializeField] float wallDistance = 0.05f;

    [Header("Physic cast")]
    [SerializeField] float maxDistance = 100;
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

    private void OnLeftButtonPressed(InputAction.CallbackContext obj) => ShootPortal(leftHand, portalLeft, false);
    private void OnRightButtonPressed(InputAction.CallbackContext obj) => ShootPortal(rightHand, portalRight, true);

    private void ShootPortal(Transform pointer, Portal portal, bool inverseRotation)
    {
        if (Physics.Raycast(pointer.position, pointer.forward, out RaycastHit hit, maxDistance, nonPortalLayer))
        {
            Vector3 portalPosition = hit.point + hit.normal * wallDistance;
            Quaternion portalRotation = Quaternion.LookRotation(inverseRotation ? -hit.normal : hit.normal);
            portal.transform.SetPositionAndRotation(portalPosition, portalRotation);
        }
    }
}
