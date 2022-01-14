using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float rotationSpeed;
    public bool pushObjects;

    public float rotationStartDelay = 0;

    bool canRotate;

    private void OnEnable()
    {
        Invoke("StartRotation", rotationStartDelay);
    }

    private void FixedUpdate()
    {
        if (canRotate)
        {
            transform.Rotate(0, 0, rotationSpeed);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (pushObjects)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("MovableObject"))
            {
                collision.transform.position = new Vector2(collision.transform.position.x - rotationSpeed * Time.deltaTime, collision.transform.position.y);
            }
        }
    }

    void StartRotation()
    {
        canRotate = true;
    }
}
