using System;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] Paddle paddleTop, paddleBottom;
    [SerializeField, Min(0f)] Vector2 arenaSize = new Vector2(10f, 10f);
    [SerializeField, Min(2)] int pointsToWin = 3;

    [SerializeField]
    TextMeshPro countdownText;

    [SerializeField, Min(1f)]
    float newGameDelay = 5f,
    newRoundDelay = 3f;

    float currentDelay;

    float countdownUntilNewGame;
    [SerializeField]
    LivelyCamera livelyCamera;

    delegate void StartNewGameOrRound();
    StartNewGameOrRound roundOrGame;

    void Update()
    {
        paddleBottom.Move(ball.Position.x, arenaSize.x);
        paddleTop.Move(ball.Position.x, arenaSize.x);
        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            updateGame();
        }
        else
        {
            UpdateCountdown();
        }
    }

    void updateGame()
    {
        ball.Move();
        BounceXIfNeeded(ball.Position.x);
        BounceYIfNeeded();
        ball.UpdateVisualization();
    }

    void BounceXIfNeeded(float x)
    {
        float xSize = arenaSize.x - ball.BallHalfSize;
        if (x < -xSize)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xSize);
        }
        else if (x > xSize)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(xSize);
        }
    }

    void BounceYIfNeeded()
    {
        float ySize = arenaSize.y - ball.BallHalfSize;
        if (ball.Position.y < -ySize)
        {
            BounceY(-ySize, paddleBottom, paddleTop);
        }
        else if (ball.Position.y > ySize)
        {
            BounceY(ySize, paddleTop, paddleBottom);
        }
    }

    void BounceY(float boundary, Paddle defender, Paddle attacker)
    {
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);
        if (defender.HitBall(bounceX, ball.BallHalfSize, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else
        {
            livelyCamera.JostleY();
            if (attacker.ScorePoint(pointsToWin))
            {
                EndGame();
            } else
            {
                EndRound();
            }
        }
    }

    void Awake()
    {
        roundOrGame = StartNewGame;
        countdownUntilNewGame = newGameDelay;
        currentDelay = newGameDelay;
    }

    void StartNewGame()
    {
        ball.StartNewGame();
        paddleBottom.StartNewGame();
        paddleTop.StartNewGame();
    }

    void StartNewRound()
    {
        ball.StartNewGame();
        paddleBottom.StartNewRound();
        paddleTop.StartNewRound();
    }

    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("KONEC IGRE");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
        Debug.Log("End game");
        currentDelay = newGameDelay;
        roundOrGame = StartNewGame;
    }

    void EndRound()
    {
        countdownText.gameObject.SetActive(true);
        countdownUntilNewGame = newRoundDelay;
        ball.EndGame();
        Debug.Log("End round");
        currentDelay = newRoundDelay;
        Debug.Log("currentDelay");
        Debug.Log(currentDelay);
        roundOrGame = StartNewRound;
    }

    void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            roundOrGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            Debug.Log($"displayValue: {displayValue}");
            if (displayValue < currentDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }
}
