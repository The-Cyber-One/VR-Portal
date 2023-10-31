using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [SerializeField] Transform playerHead, leftHand, rightHand;
    [SerializeField] ConfigurableJoint leftHandJoint, rightHandJoint;
    [SerializeField] CapsuleCollider bodyCollider;

    private void FixedUpdate()
    {
        //bodyCollider.center = new Vector3(playerHead.localPosition.x, playerHead.localPosition.y - bodyCollider.height / 2, playerHead.localPosition.z);

        leftHandJoint.targetPosition = leftHand.localPosition;
        leftHandJoint.targetRotation = leftHand.localRotation;

        rightHandJoint.targetPosition = rightHand.localPosition;
        rightHandJoint.targetRotation = rightHand.localRotation;
    }
}
