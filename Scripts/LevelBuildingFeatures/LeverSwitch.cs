using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverSwitch : MonoBehaviour
{
    public bool startsLeft;
    bool switchDirectionRight;

    public UnityEvent switchLeft, switchRight;

    private void OnEnable()
    {
        if (startsLeft && switchDirectionRight)
        {
            SwitchLeft();
        }
        else if (!startsLeft && !switchDirectionRight)
        {
            SwitchRight();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D collisionRb = collision.GetComponent<Rigidbody2D>();

            if (collisionRb.velocity.x > 0)
            {
                SwitchRight();
            }
            else if (collisionRb.velocity.x < 0)
            {
                SwitchLeft();
            }
        }
    }

    void SwitchLeft()
    {
        switchLeft.Invoke();
    }

    void SwitchRight()
    {
        switchRight.Invoke();
    }
}
