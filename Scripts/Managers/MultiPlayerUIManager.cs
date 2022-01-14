using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiPlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI playerCountText;
    int playerCount;

    public List<TextMeshProUGUI> pointsTexts;

    public List<PlayerPositionManager> joo;
    List<GameObject> players;

    public TextMeshProUGUI winnerText;

    public void PlayerJoin(PlayerPositionManager player)
    {
        players = CouchPartyManager.Instance.allPlayers;


        print("PLAYERJOINED");
        playerCountText.text = "Players: " + players.Count.ToString();

        //players.Add(player);

        for (int i = 0; i < players.Count; i++)
        {
            pointsTexts[i].gameObject.SetActive(true);
        }

        UpdatePoints();

        EnablePointTexts();
    }

    public void PlayerLeave(PlayerPositionManager player)
    {
        players = CouchPartyManager.Instance.allPlayers;

        print("PLAYERLEFT");
        playerCountText.text = "Players: " + players.ToString();

        //players.Remove(player);

        UpdatePoints();

        for (int i = 0; i < players.Count; i++)
        {
            if (i + 1 == players.Count)
            {
                pointsTexts[i + 1].gameObject.SetActive(false);
            }
        }

        EnablePointTexts();
    }

    void EnablePointTexts()
    {
        float textYpos = 40;

        switch (players.Count)
        {
            case 1:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(0, textYpos);
                break;
            case 2:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-100, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(100, textYpos);
                break;
            case 3:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-200, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(0, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(200, textYpos);
                break;
            case 4:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-300, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(-100, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(100, textYpos);
                pointsTexts[3].rectTransform.anchoredPosition = new Vector2(300, textYpos);
                break;
            case 5:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-400, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(-200, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(0, textYpos);
                pointsTexts[3].rectTransform.anchoredPosition = new Vector2(200, textYpos);
                pointsTexts[4].rectTransform.anchoredPosition = new Vector2(400, textYpos);
                break;
            case 6:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-500, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(-300, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(-100, textYpos);
                pointsTexts[3].rectTransform.anchoredPosition = new Vector2(100, textYpos);
                pointsTexts[4].rectTransform.anchoredPosition = new Vector2(300, textYpos);
                pointsTexts[5].rectTransform.anchoredPosition = new Vector2(500, textYpos);
                break;
            case 7:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-600, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(-400, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(-200, textYpos);
                pointsTexts[3].rectTransform.anchoredPosition = new Vector2(0, textYpos);
                pointsTexts[4].rectTransform.anchoredPosition = new Vector2(200, textYpos);
                pointsTexts[5].rectTransform.anchoredPosition = new Vector2(400, textYpos);
                pointsTexts[6].rectTransform.anchoredPosition = new Vector2(600, textYpos);
                break;
            case 8:
                pointsTexts[0].rectTransform.anchoredPosition = new Vector2(-700, textYpos);
                pointsTexts[1].rectTransform.anchoredPosition = new Vector2(-500, textYpos);
                pointsTexts[2].rectTransform.anchoredPosition = new Vector2(-300, textYpos);
                pointsTexts[3].rectTransform.anchoredPosition = new Vector2(-100, textYpos);
                pointsTexts[4].rectTransform.anchoredPosition = new Vector2(100, textYpos);
                pointsTexts[5].rectTransform.anchoredPosition = new Vector2(300, textYpos);
                pointsTexts[6].rectTransform.anchoredPosition = new Vector2(500, textYpos);
                pointsTexts[7].rectTransform.anchoredPosition = new Vector2(700, textYpos);
                break;

            default:
                break;
        }
    }

    public void UpdatePoints()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int playerNumber = i + 1;
            pointsTexts[i].text = "P" + playerNumber + ": " + players[i].GetComponent<PlayerPositionManager>().points.ToString();
        }
    }

    public IEnumerator DeclareWinner(int winnerNumber, GameObject winner)
    {
        winnerText.gameObject.SetActive(true);
        winnerText.text = "P" + winnerNumber + " Is The Wiener";

        PlayerController winnerController = winner.GetComponent<PlayerController>();

        yield return new WaitForSeconds(0.075f);

        winnerController.canMove = false;
        winner.transform.position = new Vector2(0, 3);
        winnerController.ResetMovementRestrictions(2.5f, true, true, true);

        yield return new WaitForSeconds(3);

        winnerText.gameObject.SetActive(false);

        yield return null;
    }

    private static MultiPlayerUIManager instance;

    public static MultiPlayerUIManager Instance { get { return instance; } }

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
