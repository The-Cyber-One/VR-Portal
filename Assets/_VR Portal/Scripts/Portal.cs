using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal otherPortal;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform leftEye, rightEye, center;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem growParticles;

    InputDevice _hmd;
    Camera _portalCamLeft, _portalCamRight;
    RenderTexture _portalTextureLeft, _portalTextureRight;
    List<PortalTraveller> _travellers = new();
    Collider _wallCollider;

    public int Layer => meshRenderer.gameObject.layer;
    public Transform PortalScreen => meshRenderer.transform;

    private void OnEnable()
    {
        InputDevices.deviceConnected += DeviceConnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= DeviceConnected;
    }

    private void Start()
    {
        Collider[] wall = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, 0.1f, wall, ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));
        _wallCollider = wall[0];

        if (_wallCollider == null)
        {
            Debug.LogWarning("Portal didn't find starter wall");
        }
    }

    private void FixedUpdate()
    {
        for (int i = _travellers.Count - 1; i >= 0; i--)
        {
            var traveller = _travellers[i];
            Vector3 offsetFromPortal = transform.position - traveller.transform.position;

            int side = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int previousSide = System.Math.Sign(Vector3.Dot(traveller.PreviousOffsetFromPortal, transform.forward));

            if (side != previousSide)
            {
                traveller.Teleport(this, otherPortal);
            }
            else
            {
                traveller.PreviousOffsetFromPortal = offsetFromPortal;
            }
        }

        ProtectScreenFromClipping();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PortalTraveller traveller))
        {
            TravellerEnterdPortal(traveller);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PortalTraveller traveller))
        {
            TravellerExitedPortal(traveller);
        }
    }

    private void DeviceConnected(InputDevice obj)
    {
        if (obj.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
        {
            _hmd = obj;
            StartCoroutine(C_CreatePortalCam());
        }
    }

    private IEnumerator C_CreatePortalCam()
    {
        float fov = playerCam.fieldOfView;
        yield return new WaitUntil(() => fov != playerCam.fieldOfView);
        _portalCamLeft = CreatePortalCam();
        _portalCamRight = CreatePortalCam();
    }

    private Camera CreatePortalCam()
    {
        Camera camera = new GameObject(name + " Cam").AddComponent<Camera>();
        camera.enabled = false;
        camera.transform.parent = transform;

        // Copy the player cam data to portal cam
        camera.stereoTargetEye = StereoTargetEyeMask.None;
        camera.fieldOfView = playerCam.fieldOfView;
        camera.nearClipPlane = playerCam.nearClipPlane;
        camera.farClipPlane = playerCam.farClipPlane;
        camera.forceIntoRenderTexture = true;
        camera.targetDisplay = 1;

        // Don't render the portal itself
        camera.cullingMask = ~LayerMask.GetMask(LayerMask.LayerToName(otherPortal.Layer));
        return camera;
    }

    // Called before player cam renders
    public void Render()
    {
        if (_portalCamLeft == null || _portalCamRight == null)
            return;

        if (!VisibleFromCamera(meshRenderer, playerCam))
        {
            var testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.red);
            testTexture.Apply();
            meshRenderer.material.SetTexture("_LeftTex", testTexture);
            return;
        }
        meshRenderer.material.SetTexture("_LeftTex", _portalTextureLeft);

        CreateCamTextures();

        // Go from otherPortal position in world > otherPortal position in this portal > otherPortal position displaced by camera position in this portal
        var portalMatrixLeft = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * leftEye.localToWorldMatrix;
        _portalCamLeft.transform.SetPositionAndRotation(portalMatrixLeft.GetPosition(), portalMatrixLeft.rotation);
        var portalMatrixRight = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * rightEye.localToWorldMatrix;
        _portalCamRight.transform.SetPositionAndRotation(portalMatrixRight.GetPosition(), portalMatrixRight.rotation);

        try
        {
            _portalCamLeft.projectionMatrix = ObliqueProjection(_portalCamLeft);
            _portalCamRight.projectionMatrix = ObliqueProjection(_portalCamRight);
        }
        catch { }

        _portalCamLeft.Render();
        _portalCamRight.Render();
    }

    private void CreateCamTextures()
    {
        if (!_hmd.isValid || XRSettings.eyeTextureWidth == 0 || XRSettings.eyeTextureHeight == 0)
            return;

        int eyeWidth = XRSettings.eyeTextureWidth, eyeHeight = XRSettings.eyeTextureHeight;

        // Don't create new texture if it already is correct
        if (_portalTextureLeft != null && _portalTextureLeft.width == eyeWidth && _portalTextureLeft.height == eyeHeight
            && _portalTextureLeft != null && _portalTextureRight.width == eyeWidth && _portalTextureRight.height == eyeHeight)
            return;

        if (_portalTextureLeft != null)
        {
            _portalTextureLeft.Release();
        }
        if (_portalTextureRight != null)
        {
            _portalTextureRight.Release();
        }

        Debug.Log(eyeWidth + ", " + eyeHeight);
        _portalTextureLeft = new RenderTexture(eyeWidth, eyeHeight, 24, RenderTextureFormat.Default);
        _portalTextureRight = new RenderTexture(eyeWidth, eyeHeight, 24, RenderTextureFormat.Default);
        _portalCamLeft.targetTexture = _portalTextureLeft;
        _portalCamRight.targetTexture = _portalTextureRight;
        meshRenderer.material.SetTexture("_LeftTex", _portalTextureLeft);
        meshRenderer.material.SetTexture("_RightTex", _portalTextureRight);
    }

    private Matrix4x4 ObliqueProjection(Camera cam)
    {
        Transform clipPlane = otherPortal.meshRenderer.transform;
        Vector3 clipPosition = clipPlane.position; // TODO: try displacing position to the portal screen mesh edge
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPosition - cam.transform.position));

        Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint(clipPosition);
        Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDistance = -Vector3.Dot(camSpacePos, camSpaceNormal);
        Vector4 clipPlaneCamSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDistance);

        return cam.CalculateObliqueMatrix(clipPlaneCamSpace);
    }

    public void MovePortal(Collider wall, Vector3 position, Quaternion rotation)
    {
        _wallCollider = wall;
        animator.Play("Grow");
        growParticles.Play();
        transform.SetPositionAndRotation(position, rotation);
    }

    public void TravellerEnterdPortal(PortalTraveller traveller)
    {
        if (_travellers.Contains(traveller))
            return;
        traveller.PreviousOffsetFromPortal = transform.position - traveller.transform.position;
        _travellers.Add(traveller);
        traveller.SetPortalCollision(true, _wallCollider);
    }

    public void TravellerExitedPortal(PortalTraveller traveller)
    {
        if (!_travellers.Contains(traveller))
            return;
        _travellers.Remove(traveller);
        traveller.SetPortalCollision(false, _wallCollider);
    }

    private void ProtectScreenFromClipping()
    {
        Transform closestEye = (leftEye.position - transform.position).sqrMagnitude > (rightEye.position - transform.position).sqrMagnitude
            ? rightEye
            : leftEye;

        Transform portalMesh = meshRenderer.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - closestEye.position) > 0;
        portalMesh.localPosition = (camFacingSameDirAsPortal ? 0.6f : -0.6f) * portalMesh.localScale.z * Vector3.forward;
    }

    private static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }
}
