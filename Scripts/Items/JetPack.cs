using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    private bool isFlying = false;
    private float jetpackForce = 40f;
    public float timer = 0;
    public float fuelBurnRate = 40f;

    public float explosionForce;

    bool hasHolder;

    Rigidbody2D holderRb;
    PlayerController holderController;

    Item item;

    // Start is called before the first frame update
    void Start()
    {
        hasHolder = false;

        item = GetComponent<Item>();

        if (CouchPartyManager.Instance.slowMode)
        {
            explosionForce *= 0.7f;
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

            holderController.useItem.AddListener(UseItem);
            holderController.stopUsingItem.AddListener(StopUsingItem);
        }
    }
    private void FixedUpdate()
    {
        if (isFlying == true && timer <= 3)
        {
            timer += Time.deltaTime;
            holderRb.AddForce(Vector2.up * jetpackForce);
        }
    }

    void UseItem()
    {
        isFlying = true;
    }

    void StopUsingItem()
    {
        isFlying = false;

        item.Use();
        timer = 0;
    }
    private void OnDestroy()
    {
        if (item.holder != null)
        {
            holderRb.velocity = new Vector2(holderRb.velocity.x, 0);
            holderRb.AddForce(new Vector2(holderController.movementDirection * explosionForce, explosionForce), ForceMode2D.Impulse);
        }
    }
}
