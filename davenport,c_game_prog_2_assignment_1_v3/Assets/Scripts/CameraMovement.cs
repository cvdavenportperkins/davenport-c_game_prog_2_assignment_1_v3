using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float Speed = 0.125f;
    public Vector3 offset;

    void LateUpdate() // LateUpdate to make sure it is executed last
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Speed);
            transform.position = smoothedPosition;
        }
    }
}
