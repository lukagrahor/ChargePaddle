using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] Paddle paddleTop, paddleBottom;
    [SerializeField, Min(0f)] Vector2 arenaSize = new Vector2(10f, 10f);
    // Start is called before the first frame update
    void Start()
    {
        ball.StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        paddleBottom.Move(ball.Position.x, arenaSize.x);
        paddleTop.Move(ball.Position.x, arenaSize.x);
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
            BounceY(-ySize, paddleBottom);
        }
        else if (ball.Position.y > ySize)
        {
            BounceY(ySize, paddleTop);
        }
    }

    void BounceY(float boundary, Paddle defender)
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
    }
}
