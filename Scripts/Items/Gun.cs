using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Rigidbody2D holderRb;
    PlayerController holderController;

    bool hasHolder;

    public float kickBack;
    public bool resetVelocityOnKickBack;
    public bool resetXVelocity;

    public Transform shootPoint;

    public GameObject bulletPrefab;
    public int amountOfBullets;
    public float spread;

    public float fireRate;
    bool canShoot;

    Item item;

    AudioSource ads;
    public AudioClip shotSound;

    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
        hasHolder = false;

        item = GetComponent<Item>();

        if (CouchPartyManager.Instance.slowMode)
        {
            kickBack *= 0.7f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (item.givenToPlayer && !hasHolder)
        {
            hasHolder = true;

            holderController = item.holder.GetComponent<PlayerController>();
            holderRb = item.holder.GetComponent<Rigidbody2D>();
            ads = item.holder.GetComponent<AudioSource>();

            holderController.useItem.AddListener(StartShot);
        }
    }

    void StartShot()
    {
        if (canShoot)
        {
            StartCoroutine(Shoot());
            ads.PlayOneShot(shotSound);
        } 
    }

    IEnumerator Shoot()
    {
        holderController.ForceLowJump();

        canShoot = false;

        if (kickBack != 0)
        {
            if (resetVelocityOnKickBack)
            {
                if (resetXVelocity)
                {
                    holderRb.velocity = new Vector2(holderRb.velocity.x / 2, 0);
                }
                else
                {
                    holderRb.velocity = new Vector2(holderRb.velocity.x, 0);
                }  
            }
            
            holderRb.AddRelativeForce(-transform.right.normalized * kickBack, ForceMode2D.Impulse);
        }

        for (int i = 0; i < amountOfBullets; i++)
        {
            float randomSpread = Random.Range(-spread, spread);
            Instantiate(bulletPrefab, shootPoint.position, Quaternion.Euler(0, 0, transform.eulerAngles.z + randomSpread));
        }

        item.Use();

        yield return new WaitForSeconds(1 / fireRate);

        canShoot = true;
    }
}
