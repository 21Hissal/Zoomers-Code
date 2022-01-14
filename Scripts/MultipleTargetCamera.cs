using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    //public List<Transform> targets;

    public Vector3 offset;
    float smoothTime = .5f;

    Vector3 velocity;

    public Transform target;

    public float size;

    public bool shrink;

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void GetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void FixedUpdate()
    {
        if (shrink)
        {
            cam.orthographicSize -= 0.002f;
        }

        size = cam.orthographicSize;

        if (cam.orthographicSize < 3.5f)
        {
            smoothTime = size / 25;
        }
        else
        {
            smoothTime = size / 18;
        }
        

        //if (targets.Count == 0) return;

        //Vector3 centerPoint = GetCenterPoint();

        //Vector3 targetPosition = new Vector3(centerPoint.x + targets[1].position.x / divisionX, centerPoint.y + targets[1].position.y / divisionY, 0);

        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    //Vector3 GetCenterPoint()
    //{
    //    if (targets.Count == 1)
    //    {
    //        return targets[0].position;
    //    }

    //    var bounds = new Bounds(targets[0].position, Vector3.zero);
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        bounds.Encapsulate(targets[i].position);
    //    }

    //    return bounds.center;
    //}
}
