using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    public int currentLap, currentCheckPoint, position, points;

    GameObject cam;

    bool destroying;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void SetPosition(int checkPoint)
    {
        currentCheckPoint = checkPoint;

        List<GameObject> players = CouchPartyManager.Instance.allPlayers;

        position = players.Count;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].gameObject != this.gameObject)
            {
                PlayerPositionManager othersPosition = players[i].GetComponent<PlayerPositionManager>();

                if (othersPosition.currentLap < currentLap)
                {
                    position--;
                }
                else if (othersPosition.currentLap == currentLap && othersPosition.currentCheckPoint < currentCheckPoint)
                {
                    position--;
                }
            }
        }
    }

    private void Update()
    {
        MultipleTargetCamera camScript = cam.GetComponent<MultipleTargetCamera>();

        float camDistX = cam.transform.position.x + camScript.size * 16 / 9 + 1;
        float negCamDistX = cam.transform.position.x - camScript.size * 16 / 9 - 1;
        float camDistY = cam.transform.position.y + camScript.size + 1;
        float negCamDistY = cam.transform.position.y - camScript.size - 1;

        if (transform.position.x > camDistX || transform.position.x < negCamDistX || transform.position.y > camDistY || transform.position.y < negCamDistY)
        {
            if (!destroying)
            {
                destroying = true;
                Invoke("DisablePlayer", 1.5f);
            }
        }
        else if (transform.position.x < camDistX || transform.position.x > negCamDistX || transform.position.y < camDistY || transform.position.y > negCamDistY)
        {
            if (destroying)
            {
                destroying = false;
                CancelInvoke("DisablePlayer");
            }
        }
    }

    void DisablePlayer()
    {
        GetComponent<PlayerController>().stopUsingItem.Invoke();

        Destroy(GetComponent<PlayerItemManager>().itemInHand);

        CouchPartyManager.Instance.CountScore();
        gameObject.SetActive(false);
    }
}
