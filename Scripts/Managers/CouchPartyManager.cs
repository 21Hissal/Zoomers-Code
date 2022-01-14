using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CouchPartyManager : MonoBehaviour
{
    int currentMap;
    public List<GameObject> maps;
    public List<float> camSizes;
    float currentMapCamSize;

    [HideInInspector]
    public Vector2 spawnPos;

    public MultipleTargetCamera camScript;
    public Camera cam;

    public List<GameObject> allPlayers;

    public bool slowMode;

    public void SelectMap(int mapNumber)
    {
        currentMap = mapNumber;
    }

    public void StartGame()
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("DestroyOnStart");

        for (int i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }

        for (int i = 0; i < maps.Count; i++)
        {
            if (currentMap == i)
            {
                cam.orthographicSize = camSizes[i];
                currentMapCamSize = camSizes[i];
            }
        }

        Invoke("StartShrinkingCamera", 5);

        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //allPlayers = new List<GameObject>(players);

        maps[currentMap].SetActive(true);
        maps[0].SetActive(false);

        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerController playerController = allPlayers[i].GetComponent<PlayerController>();
            PlayerItemManager playerItems = allPlayers[i].GetComponent<PlayerItemManager>();
            PlayerPositionManager playerPosition = allPlayers[i].GetComponent<PlayerPositionManager>();

            allPlayers[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            allPlayers[i].transform.position = spawnPos;
            playerController.canMove = false;
            playerController.ResetMovementRestrictions(1, true, true, true);
            playerPosition.currentCheckPoint = -1;
            playerPosition.position = 1;
            playerPosition.points = 0;

            if (playerController.movementDirection == -1)
            {
                playerController.Flip();
            }

            playerController.stopUsingItem.Invoke();

            Destroy(playerItems.itemInHand);
            playerItems.item = null;

            MultiPlayerUIManager.Instance.UpdatePoints();
        }
    }

    void StartShrinkingCamera()
    {
        camScript.shrink = true;
    }

    public void CountScore()
    {
        StartCoroutine(CheckPlayers());
    }

    IEnumerator CheckPlayers()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject activePlayer;
        PlayerController activePlayerController;
        PlayerPositionManager activePlayerPositionManager;
        PlayerItemManager activePlayerItemManager;

        PlayerController playerController;
        PlayerPositionManager playerPositionManager;

        int playerCount = allPlayers.Count;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (!allPlayers[i].activeInHierarchy)
            {
                print("playercount down");
                playerCount--;
            }
        }

        if (playerCount == 1)
        {
            CancelInvoke("StartShrinkingCamera");
            camScript.shrink = false;

            for (int i = 0; i < allPlayers.Count; i++)
            {
                print(i + "Looking For Active");

                if (allPlayers[i].activeInHierarchy)
                {
                    print("found active");

                    activePlayer = allPlayers[i];
                    activePlayerController = activePlayer.GetComponent<PlayerController>();
                    activePlayerPositionManager = activePlayer.GetComponent<PlayerPositionManager>();
                    activePlayerItemManager = activePlayer.GetComponent<PlayerItemManager>();

                    activePlayerPositionManager.points++;

                    activePlayerController.stopUsingItem.Invoke();
                    Destroy(activePlayerItemManager.itemInHand);

                    MultiPlayerUIManager.Instance.UpdatePoints();

                    for (int i2 = 0; i2 < allPlayers.Count; i2++)
                    {
                        print(i2);

                        playerController = allPlayers[i2].GetComponent<PlayerController>();
                        playerPositionManager = allPlayers[i2].GetComponent<PlayerPositionManager>();

                        allPlayers[i2].SetActive(true);
                        allPlayers[i2].transform.position = activePlayer.transform.position;

                        playerController.canMove = false;
                        playerController.dashing = false;
                        playerController.canDash = true;
                        if (playerController.movementDirection != activePlayerController.movementDirection)
                        {
                            playerController.Flip();
                        }

                        playerPositionManager.currentLap = activePlayerPositionManager.currentLap;
                        playerPositionManager.currentCheckPoint = activePlayerPositionManager.currentCheckPoint;

                        if (i2 == allPlayers.Count - 1)
                        {
                            print("waiting");
                            yield return new WaitForSeconds(1);

                            for (int i3 = 0; i3 < allPlayers.Count; i3++)
                            {
                                print(i3 + " canmove");

                                playerController = allPlayers[i3].GetComponent<PlayerController>();

                                playerController.ResetMovementRestrictions(0.05f, true, true, true);
                            }

                            Invoke("StartShrinkingCamera", 5);
                            cam.orthographicSize = currentMapCamSize;

                            if (activePlayerPositionManager.points == 3)
                            {
                                EndGame();
                            }
                        }
                    }

                    yield break;
                }
            }
        }
        else if (playerCount == 0)
        {
            CancelInvoke("StartShrinkingCamera");
            camScript.shrink = false;

            MultiPlayerUIManager.Instance.UpdatePoints();

            for (int i2 = 0; i2 < allPlayers.Count; i2++)
            {
                print(i2);

                playerController = allPlayers[i2].GetComponent<PlayerController>();
                playerPositionManager = allPlayers[i2].GetComponent<PlayerPositionManager>();

                allPlayers[i2].SetActive(true);
                allPlayers[i2].transform.position = Vector2.zero;

                playerController.canMove = false;
                playerController.dashing = false;
                playerController.canDash = true;
                if (playerController.movementDirection == -1)
                {
                    playerController.Flip();
                }

                playerPositionManager.currentLap = 0;
                playerPositionManager.currentCheckPoint = -1;

                if (i2 == allPlayers.Count - 1)
                {
                    print("waiting");
                    yield return new WaitForSeconds(1);

                    for (int i3 = 0; i3 < allPlayers.Count; i3++)
                    {
                        print(i3 + " canmove");

                        playerController = allPlayers[i3].GetComponent<PlayerController>();

                        playerController.ResetMovementRestrictions(0.1f, true, true, true);
                    }

                    Invoke("StartShrinkingCamera", 5);
                    cam.orthographicSize = currentMapCamSize;
                }
            }

            yield break;
        }

        yield return null;
    }

    public void EndGame()
    {
        CancelInvoke("StartShrinkingCamera");
        camScript.shrink = false;

        camScript.target = null;
        camScript.transform.position = new Vector3(0, 0, -10);

        cam.orthographicSize = camSizes[0];
        currentMapCamSize = camSizes[0];

        maps[currentMap].SetActive(false);
        maps[0].SetActive(true);

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            allPlayers[i].transform.position = spawnPos;
        }

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].GetComponent<PlayerPositionManager>().points == 3)
            {
                StartCoroutine(MultiPlayerUIManager.Instance.DeclareWinner(i + 1, allPlayers[i]));
            }
        }
    }

    public void ToggleSlowMode(Toggle toggle)
    {
        slowMode = toggle.isOn;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerController playerController = allPlayers[i].GetComponent<PlayerController>();
            playerController.SetMode(slowMode);
        }
    }

    private static CouchPartyManager instance;

    public static CouchPartyManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
