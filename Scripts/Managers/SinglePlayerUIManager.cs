using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SinglePlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI velocityText;
    public Rigidbody2D playerRb;
    public PlayerController playerMovementScript;

    public TextMeshProUGUI timerText;
    float time;

    public bool countTime;

    private void Start()
    {
        countTime = true;

        time = 0;
        InvokeRepeating("Timer", 1, 0.01f);
    }

    void FixedUpdate()
    {
        float playerVelocity = Mathf.Abs(playerRb.velocity.x);
        float roundedVelocity = Mathf.Round(playerVelocity * 100) / 100;
        float roundedNegativeVelocity = roundedVelocity * -1;

        if (playerMovementScript.movementDirection == 1 && playerRb.velocity.x > 0 || playerMovementScript.movementDirection == -1 && playerRb.velocity.x < 0)
        {
            velocityText.text = roundedVelocity.ToString();
        }
        else
        {
            velocityText.text = roundedNegativeVelocity.ToString();
        }
    }

    void Timer()
    {
        if (countTime)
        {
            time += 0.01f;

            float timeRounded = Mathf.Round(time);

            timerText.text = "Time: " + timeRounded.ToString();
        }
    }

    private static SinglePlayerUIManager instance;

    public static SinglePlayerUIManager Instance { get { return instance; } }

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
