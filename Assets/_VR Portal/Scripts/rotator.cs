using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    [SerializeField] float angle = 5;
    void Update()
    {
        transform.Rotate(Vector3.forward, angle);
    }
}
