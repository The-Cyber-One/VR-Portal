using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 1;

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), rotationAngle);
    }
}
