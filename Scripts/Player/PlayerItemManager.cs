using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject itemInHand;

    public List<GameObject> items;

    PlayerController controllerScript;

    [HideInInspector]
    public Item item;

    private void Start()
    {
        itemInHand = null;
        controllerScript = GetComponent<PlayerController>();
    }

    public void GiveItem(int itemToGive)
    {
        itemInHand = Instantiate(items[itemToGive], transform.position, transform.rotation);

        item = itemInHand.GetComponent<Item>();

        item.holder = gameObject;
        item.givenToPlayer = true;

        if (controllerScript.movementDirection == -1)
        {
            item.Flip();
        }
    }

    private void Update()
    {
        if (item != null)
        {
            item.aim = controllerScript.aim;
        }
    }

    private void OnDisable()
    {
        itemInHand = null;
    }
}
