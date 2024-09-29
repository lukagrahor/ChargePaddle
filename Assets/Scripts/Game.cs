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
    float newGameDelay = 3f;

    float countdownUntilNewGame;

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
            ball.BounceX(-xSize);
        }
        else if (x > xSize)
        {
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

        ball.BounceY(boundary);
        if(defender.HitBall(bounceX, ball.BallHalfSize, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else if (attacker.ScorePoint(pointsToWin))
        {
            EndGame();
        }
    }

    void Awake() => countdownUntilNewGame = newGameDelay;

    void StartNewGame()
    {
        ball.StartNewGame();
        paddleBottom.StartNewGame();
        paddleTop.StartNewGame();
    }

    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("KONEC IGRE");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }

    void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if (displayValue < newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }
}
