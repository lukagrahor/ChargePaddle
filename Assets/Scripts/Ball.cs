using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    maxXSpeed = 20f,
    startXSpeed = 8f,
    constantYSpeed = 10f,
    halfSize = 0.5f;

    Vector2 position, velocity;
    // Start is called before the first frame update
    void Start()
    {
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();
        velocity = new Vector2(startXSpeed, -constantYSpeed);
    }

    public void UpdateVisualization() => transform.localPosition = new Vector3(position.x, 0, position.y);

    public void Move() => position += Time.deltaTime * velocity;

    public void BounceX(float boundary)
    {
        position.x = 2 * boundary - position.x;
        velocity.x = -velocity.x;
    }

    public void BounceY(float boundary)
    {
        position.y = 2 * boundary - position.y;
        velocity.y = -velocity.y;
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    public float BallHalfSize => halfSize;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;
}
