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
        ball.Move();
        BounceXIfNeeded();
        BounceYIfNeeded();
        ball.UpdateVisualization();
    }

    void BounceXIfNeeded()
    {
        float xSize = arenaSize.x - ball.BallHalfSize;
        if (ball.Position.x < -xSize)
        {
            ball.BounceX(-xSize);
        }
        else if (ball.Position.x > xSize)
        {
            ball.BounceX(xSize);
        }
    }

    void BounceYIfNeeded()
    {
        float ySize = arenaSize.y - ball.BallHalfSize;
        if (ball.Position.y < -ySize)
        {
            ball.BounceY(-ySize);
        }
        else if (ball.Position.y > ySize)
        {
            ball.BounceY(ySize);
        }
    }
}
