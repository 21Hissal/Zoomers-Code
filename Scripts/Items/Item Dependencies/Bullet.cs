using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;

    Rigidbody2D rb;

    bool moving;

    public float selfDestructIn = 10;
    public float selfDestructInAfterColliding = 3;

    private void Start()
    {
        moving = true;

        rb = GetComponent<Rigidbody2D>();

        Invoke("DestroySelf", selfDestructIn);
        StartCoroutine(MoveBullet());
    }

    IEnumerator MoveBullet()
    {
        while (moving)
        {
            rb.velocity = (transform.rotation * Vector2.right * bulletSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(selfDestructInAfterColliding);

        DestroySelf();
    }
    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            moving = false;
        }
    }  
}
