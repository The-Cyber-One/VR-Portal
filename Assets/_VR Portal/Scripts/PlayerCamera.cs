using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Portal[] portals;

    // Unity function that will be called before the camera on the gameobject renders
    private void OnPreRender()
    {
        RenderPortals();
    }

    private void RenderPortals()
    {
        foreach (Portal portal in portals)
        {
            portal.Render();
        }
    }
}