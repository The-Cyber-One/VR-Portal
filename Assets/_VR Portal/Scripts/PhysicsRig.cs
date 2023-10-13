using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [SerializeField] Transform playerHead;
    [SerializeField] CapsuleCollider bodyCollider;
    [SerializeField] float minBodyHeight = 0.5f, maxBodyHeigh = 2f;

    private void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, minBodyHeight, maxBodyHeigh);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, playerHead.localPosition.y / 2, playerHead.localPosition.z);
    }
}
