using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    maxXSpeed = 20f,
    maxStartXSpeed = 2f,
    constantYSpeed = 10f,
    halfSize = 0.5f;

    Vector2 position, velocity;

    void Awake() => gameObject.SetActive(false);

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();
        velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
        velocity.y = -constantYSpeed;
        gameObject.SetActive(true);
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
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
