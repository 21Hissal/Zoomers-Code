using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public RespawningItem respawnableItemContainer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerItemManager itemManager;
            itemManager = collision.GetComponent<PlayerItemManager>();

            if (itemManager.itemInHand == null)
            {
                int itemToGive;
                itemToGive = Random.Range(0, itemManager.items.Count);
                itemManager.GiveItem(itemToGive);

                respawnableItemContainer.RespawnItem();
                gameObject.SetActive(false);
            }
        }
    }
}
