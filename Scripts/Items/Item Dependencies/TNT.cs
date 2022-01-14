using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    public float bulletSpeed;
    public float explosionForce;

    Rigidbody2D rb;

    bool hasHitGround;
    bool exploding;

    public float selfDestructIn = 10;
    public float selfDestructInAfterColliding = 3;

    AudioSource ads;
    public AudioClip explosionSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ads = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(DestroySelf(selfDestructIn));

        rb.velocity = (transform.rotation * Vector2.right * bulletSpeed );

        float randomTorque = Random.Range(-10, 10);
        rb.AddTorque(randomTorque, ForceMode2D.Impulse);

        Invoke("EnableCollider", 0.025f);
    }

    void EnableCollider()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    IEnumerator DestroySelf(float destroyIn)
    {
        yield return new WaitForSeconds(destroyIn);

        if (!exploding)
        {
            exploding = true;

            Destroy(GetComponent<Rigidbody2D>());
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;

            ads.PlayOneShot(explosionSound);

            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
        }

        yield break;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(DestroySelf(selfDestructInAfterColliding));

            if (gameObject.GetComponent<Rigidbody2D>() != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            
            hasHitGround = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.SetParent(collision.transform);
            Destroy(GetComponent<Rigidbody2D>());
            GetComponent<BoxCollider2D>().enabled = false;

            if (hasHitGround == true)
            {
                StartCoroutine(Explode(collision.transform, collision.gameObject.GetComponent<Rigidbody2D>(), 0));
            }
            else
            {
                StartCoroutine(Explode(collision.transform, collision.gameObject.GetComponent<Rigidbody2D>(), 2.5f));
            }
        }
    }

    IEnumerator Explode(Transform collisionTransform, Rigidbody2D collisionRb, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PlayerController playerController = collisionTransform.GetComponent<PlayerController>();

        playerController.hasForce = false;
        playerController.ResetMovementRestrictions(0.1f, true, false, false);

        collisionRb.AddForce(new Vector2((transform.position.x - collisionTransform.position.x) * -explosionForce ,(transform.position.y - collisionTransform.position.y) * -explosionForce ),ForceMode2D.Impulse);
        StartCoroutine(DestroySelf(0));

        yield return null;
    }
}
