using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInputManager : MonoBehaviour
{
    public int playerNumber;

    TextMeshPro playerNumberText;


    private void Awake()
    {
        print("AWAKE COLOR");

        CouchPartyManager.Instance.allPlayers.Add(gameObject);
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        playerNumber = CouchPartyManager.Instance.allPlayers.IndexOf(gameObject) + 1;

        playerNumberText = GetComponentInChildren<TextMeshPro>();
        playerNumberText.text = "P" + playerNumber;

        if (MultiPlayerUIManager.Instance != null)
        {
            MultiPlayerUIManager.Instance.PlayerJoin(GetComponent<PlayerPositionManager>());
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            switch (playerNumber)
            {
                case 1:
                    renderers[i].color = Color.blue;
                    break;
                case 2:
                    renderers[i].color = Color.red;
                    break;
                case 3:
                    renderers[i].color = Color.green;
                    break;
                case 4:
                    renderers[i].color = Color.yellow;
                    break;
                case 5:
                    renderers[i].color = Color.magenta;
                    break;
                case 6:
                    renderers[i].color = Color.gray;
                    break;
                case 7:
                    renderers[i].color = Color.white;
                    break;
                case 8:
                    renderers[i].color = Color.black;
                    break;
                default:
                    renderers[i].color = Color.black;
                    break;
            }
        }
    }

    public void DeviceLost()
    {
        if (MultiPlayerUIManager.Instance != null)
        {
            PlayerLeave();
        }
    }

    void PlayerLeave()
    {   
        CouchPartyManager.Instance.allPlayers.Remove(gameObject);
        MultiPlayerUIManager.Instance.PlayerLeave(GetComponent<PlayerPositionManager>());
        
        Destroy(gameObject);
    }
}