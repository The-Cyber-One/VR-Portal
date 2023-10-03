using System;
using System.Runtime.InteropServices;

public class ExampleGlue : CppGlue
{
#if UNITY_EDITOR
    // The Init delegate that will make the C# functions accessible in C++
    public delegate void InitDelegate(
        IntPtr logPointer,
        IntPtr csharpFunctionPointer);
#else
    // Unfortunately this part is always needed and basically the same
    [DllImport(nameof(ExampleGlue))]
    static extern void Init(
        IntPtr logPointer,
        IntPtr csharpFunctionPointer);

    // The mono behaviour functions are only needed when in use
    [DllImport(nameof(ExampleGlue))]
    protected static extern new void MonoBehaviourStart();

    [DllImport(nameof(ExampleGlue))]
    protected static extern new void MonoBehaviourUpdate();

    [DllImport(nameof(ExampleGlue))]
    protected static extern new void MonoBehaviourFixedUpdate();
#endif

    delegate int CsharpFunctionDelegate(int value);

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        InitDelegate Init = GetDelegate<InitDelegate>(libraryHandle, "Init");
#endif

        Init(
            Marshal.GetFunctionPointerForDelegate(new LogDelegate(Log)),
            Marshal.GetFunctionPointerForDelegate(new CsharpFunctionDelegate(CsharpFunction)));
    }

    // Optionaly add needed Unity functions
    void Start() => MonoBehaviourStart();
    void Update() => MonoBehaviourUpdate();
    void FixedUpdate() => MonoBehaviourFixedUpdate();

    // C# functions to be called from C++
    static int CsharpFunction(int value)
    {
        return value + 1;
    }
}
