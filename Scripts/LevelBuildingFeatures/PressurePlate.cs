using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onPress, onRelease;

    int amountOfTimesPressed;
    bool onCooldown;
    public float cooldownTime = 0.5f;

    private void OnEnable()
    {
        if (amountOfTimesPressed % 2 != 0)
        {
            onPress.Invoke();
        }

        onCooldown = false;

        amountOfTimesPressed = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PressRelease(true));
        }
    }

    IEnumerator PressRelease(bool isPress)
    {
        if (!onCooldown)
        {
            onCooldown = true;

            if (isPress)
            {
                onPress.Invoke();
                amountOfTimesPressed++;
            }
            else if (!isPress)
            {
                onRelease.Invoke();
            }

            yield return new WaitForSeconds(cooldownTime);

            onCooldown = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PressRelease(false));
        }
    }
}
