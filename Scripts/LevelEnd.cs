using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public PlayerController playerMovementScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovementScript = collision.GetComponent<PlayerController>();

            StartCoroutine(LoadMenu());
        }
    }

    IEnumerator LoadMenu()
    {
        playerMovementScript.canMove = false;
        SinglePlayerUIManager.Instance.countTime = false;

        yield return new WaitForSeconds(0.025f);

        GameManager.Instance.PauseGame();

        yield return new WaitForSecondsRealtime(1.5f);

        GameManager.Instance.ResumeGame();

        GameManager.Instance.LoadScene("Menu");
    }
}
