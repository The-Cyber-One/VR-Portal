using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [DllImport("VRPortalCpp.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Add(int a, int b);

    private void Start()
    {
        Debug.Log(Add(1, 2));
    }
}
