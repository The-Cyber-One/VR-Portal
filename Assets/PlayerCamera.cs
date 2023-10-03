using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Portal[] portals;

    private void OnPreRender()
    {
        foreach (Portal portal in portals)
        {
            portal.Render();
        }
    }
}
