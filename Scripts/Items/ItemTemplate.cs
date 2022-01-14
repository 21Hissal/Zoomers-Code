using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplate : MonoBehaviour
{
    bool hasHolder;

    Rigidbody2D holderRb;
    PlayerController holderController;

    Item item;

    AudioSource ads;
    public AudioClip itemUseSound;

    // Start is called before the first frame update
    void Start()
    {
        hasHolder = false;

        item = GetComponent<Item>();
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

            holderController.useItem.AddListener(UseItem);
            holderController.stopUsingItem.AddListener(StopUsingItem);
        }
    }

    void UseItem()
    {
        ads.PlayOneShot(itemUseSound);
    }

    void StopUsingItem()
    {
        item.Use();
    }
}
