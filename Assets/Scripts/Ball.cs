using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    constantXSpeed = 8f,
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
        velocity = new Vector2(constantXSpeed, -constantYSpeed);
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

    public float BallHalfSize => halfSize;
    public Vector2 Position => position;
}
