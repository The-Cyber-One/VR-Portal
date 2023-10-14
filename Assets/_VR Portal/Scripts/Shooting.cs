using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] Transform leftHand, rightHand;
    [SerializeField] InputAction inputActionLeft, inputActionRight;
    [SerializeField] Portal portalLeft, portalRight;

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

    private void OnLeftButtonPressed(InputAction.CallbackContext obj) => ShootPortal(leftHand, portalLeft);
    private void OnRightButtonPressed(InputAction.CallbackContext obj) => ShootPortal(rightHand, portalRight);

    private void ShootPortal(Transform pointer, Portal portal)
    {
        if (Physics.Raycast(pointer.position, pointer.forward, out RaycastHit hit, maxDistance, nonPortalLayer))
        {
            portal.MovePortal(hit);
        }
    }
}
