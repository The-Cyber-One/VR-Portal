using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CppPortalCamGlue : CppGlue // Change back to PortalCam or change the LibraryName
{
#if UNITY_EDITOR
    // The Init delegate that will make the C# functions accessible in C++
    delegate void InitDelegate(
        IntPtr logPointer,
        IntPtr getTransformPointer,
        IntPtr getPlayerCamTransformPointersPointer,
        IntPtr getLocalToWorldMatrixPointer,
        IntPtr setTransformPositionAndRotationPointer
        );
#else
    // Unfortunately this part is always needed and basically the same
    [DllImport(nameof(PortalCam))]
    static extern void Init(
        IntPtr logPointer,
        IntPtr getTransformPointer,
        IntPtr getPlayerCamTransformPointersPointer,
        IntPtr getLocalToWorldMatrixPointer,
        IntPtr setTransformPositionAndRotationPointer
        );

    // The mono behaviour functions are only needed when in use
    [DllImport(nameof(PortalCam))]
    protected static extern new void MonoBehaviourStart();

    [DllImport(nameof(PortalCam))]
    protected static extern new void MonoBehaviourUpdate();
#endif

    delegate int GetTransformDelegate();
    delegate int GetPlayerCamTransformDelegate();
    delegate Matrix4x4 GetLocalToWorldMatrixDelegate(int transformHandle);
    delegate void SetTransformPositionAndRotationDelegate(int transformHandle, Vector3 position, Quaternion rotation);

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        InitDelegate Init = GetDelegate<InitDelegate>(libraryHandle, "Init");
#endif

        Init(
            Marshal.GetFunctionPointerForDelegate(new LogDelegate(Log)),
            Marshal.GetFunctionPointerForDelegate(new GetTransformDelegate(GetTransform)),
            Marshal.GetFunctionPointerForDelegate(new GetPlayerCamTransformDelegate(GetPlayerCamTransform)),
            Marshal.GetFunctionPointerForDelegate(new GetLocalToWorldMatrixDelegate(GetLocalToWorldMatrix)),
            Marshal.GetFunctionPointerForDelegate(new SetTransformPositionAndRotationDelegate(SetTransformPositionAndRotation))
            );
    }

    [SerializeField] Transform playerCam;

    int _transformPointer, _playerCamPointer;

    // Optionaly add needed Unity functions
    void Start()
    {
        if (playerCam == null)
        {
            Debug.Log("Set the serialized fields");
            return;
        }
        _transformPointer = ObjectStore.Store(transform);
        _playerCamPointer = ObjectStore.Store(playerCam);
        MonoBehaviourStart();
    }

    void Update() => MonoBehaviourUpdate();

    // C# functions to be called from C++
    int GetTransform()
    {
        return _transformPointer;
    }
    int GetPlayerCamTransform()
    {
        return _playerCamPointer;
    }
    Matrix4x4 GetLocalToWorldMatrix(int transformHandle)
    {
        Transform transform = (Transform)ObjectStore.Get(transformHandle);
        return transform.localToWorldMatrix;
    }
    static void SetTransformPositionAndRotation(int transformHandle, Vector3 position, Quaternion rotation)
    {
        Transform transform = (Transform)ObjectStore.Get(transformHandle);
        transform.SetPositionAndRotation(position, rotation);
    }
}
