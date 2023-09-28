using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class CppGlue : MonoBehaviour
{
#if UNITY_EDITOR
    // Handle to the C++ DLL
    protected IntPtr libraryHandle;

    // C# functions that will call C++ functions
    protected delegate void MonoBehaviourStartDelegate();
    protected MonoBehaviourStartDelegate MonoBehaviourStart;

    protected delegate void MonoBehaviourUpdateDelegate();
    protected MonoBehaviourUpdateDelegate MonoBehaviourUpdate;

    protected delegate void MonoBehaviourFixedUpdateDelegate();
    protected MonoBehaviourFixedUpdateDelegate MonoBehaviourFixedUpdate;

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

    protected static T GetDelegate<T>(IntPtr libraryHandle, string functionName) where T : class
    {
        IntPtr symbol = GetProcAddress(libraryHandle, functionName);
        if (symbol == IntPtr.Zero)
            throw new Exception("Couldn't get function: " + functionName);

        return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
    }
    #endregion
#endif

    // LibraryName will be must be set by the parent. If this isn't done it will only cause errors in the editor
    protected static string LibraryName;
    string LibraryPath => $"/Scripts/Cpp/VRPortal/x64/Release/{LibraryName}.dll";

    // C# log delegate
    protected delegate void LogDelegate(string message);

    protected virtual void Awake()
    {
#if UNITY_EDITOR

        // Open native library
        libraryHandle = OpenLibrary(Application.dataPath + LibraryPath);
        MonoBehaviourStart = GetDelegate<MonoBehaviourStartDelegate>(libraryHandle, "Start");
        MonoBehaviourUpdate = GetDelegate<MonoBehaviourUpdateDelegate>(libraryHandle, "Update");
        MonoBehaviourFixedUpdate = GetDelegate<MonoBehaviourFixedUpdateDelegate>(libraryHandle, "FixedUpdate");
#endif

        // Init C++ library
        ObjectStore.Init(1024);
    }

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        CloseLibrary(libraryHandle);
        libraryHandle = IntPtr.Zero;
    }
#endif

    protected static void Log(string message)
    {
        Debug.Log(message);
    }
}