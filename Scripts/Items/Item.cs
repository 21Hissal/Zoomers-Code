using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int amountOfUses;
    public bool aimable;

    [HideInInspector]
    public bool givenToPlayer = false;

    [HideInInspector]
    public GameObject holder;

    public float offsetX, offsetY;

    [HideInInspector]
    public Quaternion aim;

    public bool comesFromBox = true;

    private void Update()
    {
        if (givenToPlayer)
        {
            if (holder != null)
            {
                transform.position = holder.transform.position + new Vector3(offsetX, offsetY, 0);
            }
            else if (holder == null)
            {
                Destroy(gameObject);
            }

            if (aimable)
            {
                Aim();
            }
        }
    }

    public void Use()
    {
        amountOfUses--;

        if (amountOfUses <= 0)
        {
            holder.GetComponent<PlayerController>().useItem.RemoveAllListeners();
            holder.GetComponent<PlayerController>().stopUsingItem.RemoveAllListeners();

            Destroy(gameObject);
        }
    }

    void Aim()
    {
        transform.rotation = aim;
    }

    public void Flip()
    {
        offsetX *= -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!comesFromBox && collision.gameObject.CompareTag("Player") && holder == null)
        {
            if (collision.GetComponent<PlayerItemManager>().itemInHand == null)
            {
                holder = collision.gameObject;

                holder.GetComponent<PlayerItemManager>().itemInHand = gameObject;
                holder.GetComponent<PlayerItemManager>().item = this;

                givenToPlayer = true;
            }
        }
    }
}
