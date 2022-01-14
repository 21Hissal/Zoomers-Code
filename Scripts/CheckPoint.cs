using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPoint;

    public float offsetX, offsetY;

    public bool isEnd;
    public int highestCheckPoint;

    public MultipleTargetCamera cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPositionManager player = collision.gameObject.GetComponent<PlayerPositionManager>();
            List<GameObject> players = CouchPartyManager.Instance.allPlayers;

            if (isEnd && player.currentCheckPoint >= highestCheckPoint)
            {
                player.currentLap++;
            }

            if (player.currentCheckPoint >= checkPoint - 2 && player.currentCheckPoint <= checkPoint + 2 || isEnd)
            {
                player.SetPosition(checkPoint);

                for (int i = 0; i < players.Count; i++)
                {
                    PlayerPositionManager allPlayersPositionManager = players[i].gameObject.GetComponent<PlayerPositionManager>();

                    allPlayersPositionManager.SetPosition(allPlayersPositionManager.currentCheckPoint);
                }

                if (player.position == 1)
                {
                    cam.GetTarget(player.GetComponent<Transform>());

                    cam.offset.x = offsetX;
                    cam.offset.y = offsetY;
                }
                else
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        PlayerPositionManager allPlayersPositionManager = players[i].gameObject.GetComponent<PlayerPositionManager>();

                        if (allPlayersPositionManager.position == 1)
                        {
                            cam.GetTarget(allPlayersPositionManager.GetComponent<Transform>());
                        }
                    }
                }
            }
        } 
    }
}
