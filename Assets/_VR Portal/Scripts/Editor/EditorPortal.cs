using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorPortal : MonoBehaviour
{
    [SerializeField] private Portal thisPortal, otherPortal;
    [SerializeField] private bool isRight;

    [ContextMenu(nameof(Align))]
    void Align()
    {
        var portalMatrixLeft = otherPortal.transform.localToWorldMatrix * thisPortal.transform.worldToLocalMatrix * SceneView.GetAllSceneCameras()[0].transform.localToWorldMatrix;
        transform.SetPositionAndRotation(portalMatrixLeft.GetPosition(), portalMatrixLeft.rotation);

        var cam = GetComponent<Camera>();
        cam.projectionMatrix = ObliqueProjection(cam);
    }

    private Matrix4x4 ObliqueProjection(Camera cam)
    {
        Transform clipPlane = otherPortal.PortalScreen;
        Vector3 clipPosition = clipPlane.position + (isRight ? -clipPlane.forward : clipPlane.forward) * 0.1f;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPosition - cam.transform.position));

        Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint(clipPosition);
        Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDistance = -Vector3.Dot(camSpacePos, camSpaceNormal);
        Vector4 clipPlaneCamSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDistance);

        return cam.CalculateObliqueMatrix(clipPlaneCamSpace);
    }
}
