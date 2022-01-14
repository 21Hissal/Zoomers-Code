using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public List<Transform> points;

    public int moveTowards = 0;
    public float platformSpeed = 1;
    public float waitTimeOnPoints;
    public bool moveForward = true;

    LineRenderer lineRend;
    public bool renderLine = true;

    public float startDelay = 0;

    public bool isSwitchble;
    public bool isInterruptable;

    bool moving;
    bool movingForward;

    private void OnEnable()
    {
        if (!isSwitchble)
        {
            Invoke("StartMoving", startDelay);
        }

        if (renderLine)
        {
            lineRend = GetComponent<LineRenderer>();

            lineRend.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                lineRend.SetPosition(i, points[i].position);
            }

            lineRend.material = new Material(Shader.Find("Sprites/Default"));

            lineRend.material.color = Color.gray;
            lineRend.startColor = Color.gray;
            lineRend.endColor = Color.gray;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        moving = false;
    }

    IEnumerator StartMoving(float timeToWait, bool moveToNextPoint)
    {
        yield return new WaitForSeconds(timeToWait);

        if (moveToNextPoint)
        {
            if (moveTowards + 1 == points.Count)
            {
                moveTowards = 0;
            }
            else
            {
                moveTowards++;
            }
        }
        else
        {
            if (moveTowards == 0)
            {
                moveTowards = points.Count - 1;
            }
            else
            {
                moveTowards--;
            }
        }

        StartCoroutine(MoveToNextPoint());
    }

    IEnumerator MoveToNextPoint()
    {
        if (!moving)
        {
            moving = true;

            while (transform.position != points[moveTowards].transform.position)
            {
                transform.position = Vector2.MoveTowards(transform.position, points[moveTowards].position, platformSpeed * Time.deltaTime);
                yield return null;
            }

            moving = false;
        }

        if (!isSwitchble)
        {
            StartCoroutine(StartMoving(waitTimeOnPoints, moveForward));
        }
    }

    public void GoToNextPoint()
    {
        if (isInterruptable && !movingForward || transform.position == points[moveTowards].transform.position)
        {
            movingForward = true;
            StartCoroutine(StartMoving(0, moveForward));
        }
        else if (isInterruptable && points.Count == 2)
        {
            movingForward = true;
            StartCoroutine(StartMoving(0, moveForward));
        }
    }

    public void GoBackToLastPoint()
    {
        if (isInterruptable && movingForward || transform.position == points[moveTowards].transform.position)
        {
            movingForward = false;
            StartCoroutine(StartMoving(0, !moveForward));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("DestroyOnStart"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("DestroyOnStart"))
        {
            collision.transform.SetParent(null);
        }
    }

    void StartMoving()
    {
        StartCoroutine(MoveToNextPoint());
    }
}
