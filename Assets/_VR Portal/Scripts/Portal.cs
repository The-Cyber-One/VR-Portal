using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal otherPortal;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform leftEye, rightEye, center;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float portalResolutionScaling = 1.0f;

    InputDevice _hmd;
    Camera _portalCamLeft, _portalCamRight;
    RenderTexture _portalTextureLeft, _portalTextureRight;

    void OnEnable()
    {
        InputDevices.deviceConnected += DeviceConnected;
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= DeviceConnected;
    }

    // Called before player cam renders
    public void Render()
    {
        if (_portalCamLeft == null || _portalCamRight == null)
            return;

        CreateCamTextures();

        // Go from otherPortal position in world > otherPortal position in this portal > otherPortal position displaced by camera position in this portal
        var portalMatrixLeft = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * leftEye.localToWorldMatrix;
        _portalCamLeft.transform.SetPositionAndRotation(portalMatrixLeft.GetPosition(), portalMatrixLeft.rotation);
        var portalMatrixRight = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * rightEye.localToWorldMatrix;
        _portalCamRight.transform.SetPositionAndRotation(portalMatrixRight.GetPosition(), portalMatrixRight.rotation);

        _portalCamLeft.Render();
        _portalCamRight.Render();
    }

    void DeviceConnected(InputDevice obj)
    {
        if (obj.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
        {
            _hmd = obj;
            StartCoroutine(C_CreatePoralCams());
        }
    }

    IEnumerator C_CreatePoralCams()
    {
        yield return null; // Initialize the player cam first
        _portalCamLeft = CreatePortalCam();
        _portalCamRight = CreatePortalCam();
    }

    Camera CreatePortalCam()
    {
        Camera camera = new GameObject(name + " Cam").AddComponent<Camera>();
        camera.transform.parent = transform;
        camera.stereoTargetEye = StereoTargetEyeMask.None;
        camera.fieldOfView = playerCam.fieldOfView;
        camera.nearClipPlane = playerCam.nearClipPlane;
        camera.farClipPlane = playerCam.farClipPlane;
        camera.targetDisplay = 1;
        camera.cullingMask = ~LayerMask.GetMask(LayerMask.LayerToName(meshRenderer.gameObject.layer));
        return camera;
    }

    void CreateCamTextures()
    {
        if (!_hmd.isValid || XRSettings.eyeTextureWidth == 0 || XRSettings.eyeTextureHeight == 0)
            return;

        // Don't create new texture if it already is correct
        if (_portalTextureLeft != null && _portalTextureLeft.width == XRSettings.eyeTextureWidth && _portalTextureLeft.height == XRSettings.eyeTextureHeight
            && _portalTextureLeft != null && _portalTextureRight.width == XRSettings.eyeTextureWidth && _portalTextureRight.height == XRSettings.eyeTextureHeight)
            return;

        if (_portalTextureLeft != null)
        {
            _portalTextureLeft.Release();
        }
        if (_portalTextureRight != null)
        {
            _portalTextureRight.Release();
        }

        _portalTextureLeft = new RenderTexture((int)(XRSettings.eyeTextureWidth * portalResolutionScaling), (int)(XRSettings.eyeTextureHeight * portalResolutionScaling), 1, RenderTextureFormat.Default);
        _portalTextureRight = new RenderTexture((int)(XRSettings.eyeTextureWidth * portalResolutionScaling), (int)(XRSettings.eyeTextureHeight * portalResolutionScaling), 1, RenderTextureFormat.Default);
        _portalCamLeft.targetTexture = _portalTextureLeft;
        _portalCamRight.targetTexture = _portalTextureRight;
        meshRenderer.material.SetTexture("_LeftTex", _portalTextureLeft);
        meshRenderer.material.SetTexture("_RightTex", _portalTextureRight);
    }
}
