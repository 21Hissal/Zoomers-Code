using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningItem : MonoBehaviour
{
    public List<GameObject> itemsToRespawn;

    public int respawnTime = 2;
    public int amountOfItemsToRespawn = 1;

    public void RespawnItem()
    {
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        int randomNumber = Random.Range(0, itemsToRespawn.Count);

        while (itemsToRespawn[randomNumber].activeInHierarchy)
        {
            randomNumber = Random.Range(0, itemsToRespawn.Count);
            print(randomNumber);
            yield return new WaitForSeconds(0.1f);
        }

        itemsToRespawn[randomNumber].SetActive(true);
    }

    private void OnEnable()
    {
        for (int i = 0; i < itemsToRespawn.Count; i++)
        {
            itemsToRespawn[i].SetActive(false);
        }

        StartCoroutine(SpawnItems());
    }

    IEnumerator SpawnItems()
    {
        for (int i = 0; i < amountOfItemsToRespawn; i++)
        {
            int randomNumber = Random.Range(0, itemsToRespawn.Count);

            while (itemsToRespawn[randomNumber].activeInHierarchy)
            {
                randomNumber = Random.Range(0, itemsToRespawn.Count);
                print(randomNumber);
                yield return null;
            }

            itemsToRespawn[randomNumber].SetActive(true);
        }
    }
}
