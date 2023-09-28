using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CppGlue : MonoBehaviour
{
#if UNITY_EDITOR
    // Handle to the C++ DLL
    public IntPtr libraryHandle;

    // The Init delegate that will make the C# functions accessible in C++
    public delegate void InitDelegate(
        IntPtr gameObjectGetTransformPointer,
        IntPtr logPointer);

    // C# functions that will call C++ functions
    public delegate void MonoBehaviourStartDelegate();
    public MonoBehaviourStartDelegate MonoBehaviourStart;

    public delegate void MonoBehaviourUpdateDelegate();
    public MonoBehaviourUpdateDelegate MonoBehaviourUpdate;
#endif

    #region Library managing
    // This region won't need any more editing
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string path);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr libraryHandle, string symbolName);

    [DllImport("kernel32")]
    public static extern bool FreeLibrary(IntPtr libraryHandle);

    public static IntPtr OpenLibrary(string path)
    {
        IntPtr handle = LoadLibrary(path);
        if (handle == IntPtr.Zero)
            throw new Exception("Couldn't open native library: " + path);

        return handle;
    }

    public static void CloseLibrary(IntPtr libraryHandle)
    {
        FreeLibrary(libraryHandle);
    }

    public static T GetDelegate<T>(IntPtr libraryHandle, string functionName) where T : class
    {
        IntPtr symbol = GetProcAddress(libraryHandle, functionName);
        if (symbol == IntPtr.Zero)
            throw new Exception("Couldn't get function: " + functionName);

        return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
    }

    [SerializeField] string libraryName;
    string LibraryPath => "/Scripts/Cpp/VRPortal/x64/Release/" + libraryName;
    #endregion

    // C# delegates
    delegate int GameObjectGetTransformDelegate(int thisHandle);
    delegate void LogDelegate(string message);

    void Awake()
    {
#if UNITY_EDITOR

        // Open native library
        libraryHandle = OpenLibrary(Application.dataPath + LibraryPath);
        InitDelegate Init = GetDelegate<InitDelegate>(libraryHandle, "Init");
        MonoBehaviourStart = GetDelegate<MonoBehaviourStartDelegate>(libraryHandle, "Start");
        MonoBehaviourUpdate = GetDelegate<MonoBehaviourUpdateDelegate>(libraryHandle, "Update");

#endif

        // Init C++ library
        ObjectStore.Init(1024);
        Init(
            Marshal.GetFunctionPointerForDelegate(new GameObjectGetTransformDelegate(GameObjectGetTransform)),
            Marshal.GetFunctionPointerForDelegate(new LogDelegate(Log)));
    }

    void Start()
    {
        MonoBehaviourStart();
    }

    void Update()
    {
        MonoBehaviourUpdate();
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        CloseLibrary(libraryHandle);
        libraryHandle = IntPtr.Zero;
#endif
    }

    ////////////////////////////////////////////////////////////////
    // C# functions for C++ to call
    ////////////////////////////////////////////////////////////////
    static int GameObjectGetTransform(int thisHandle)
    {
        GameObject gameobject = (GameObject)ObjectStore.Get(thisHandle);
        Transform transform = gameobject.transform;
        return ObjectStore.Store(transform);
    }

    static void Log(string message)
    {
        Debug.Log(message);
    }
}