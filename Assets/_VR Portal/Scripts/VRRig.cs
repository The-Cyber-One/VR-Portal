using System;
using UnityEngine;

public class VRRig : MonoBehaviour
{
    [SerializeField] VRMap head, leftHand, rightHand;
    [SerializeField] Transform headConstraint;

    Vector3 _headBodyOffset;

    private void Start()
    {
        _headBodyOffset = transform.position - headConstraint.position;
    }

    private void Update()
    {
        transform.position = headConstraint.position + _headBodyOffset;
        transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}

[Serializable]
public class VRMap
{
    public Transform VRTarget, RigTarget;
    public Vector3 TrackingPositionOffset, TrackingRotationOffset;

    public void Map()
    {
        RigTarget.SetPositionAndRotation(
            VRTarget.TransformPoint(TrackingPositionOffset),
            VRTarget.rotation * Quaternion.Euler(TrackingRotationOffset));
    }
}