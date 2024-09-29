using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        minSize = 2f,
        maxSize = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;
    [SerializeField]
    TextMeshPro scoreText;
    int score;
    float size, targetingBias;

    enum Players
    {
        Player1, Player2, AI
    }

    [SerializeField]
    Players player;

    void Awake()
    {
        SetScore(0);
    }

    public void Move(float target, float arenaSize)
    {
        Vector3 p = transform.localPosition;
        p.x = player == Players.AI ? AdjustByAI(p.x, target) : AdjustByPlayers(p.x);
        float limit = arenaSize - 0.5f - size;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;
    }

    float AdjustByAI(float x, float target)
    {
        target += targetingBias * size;
        if (x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }
    
    float AdjustByPlayers(float x)
    {
        bool goRight = player == Players.Player1 ? Input.GetKey(KeyCode.RightArrow) : Input.GetKey(KeyCode.D);
        bool goLeft = player == Players.Player1 ? Input.GetKey(KeyCode.LeftArrow) : Input.GetKey(KeyCode.A);

        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        return x;
    }

    public bool HitBall(float ballX, float ballSize, out float hitFactor)
    {
        ChangeTargetingBias();
        hitFactor = (ballX - transform.localPosition.x) /
            (size + ballSize); 
        return -1f <= hitFactor && hitFactor <= 1f;
    }

    void SetScore(int newScore, float pointsToWin = 10f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);

        SetSize(Mathf.Lerp(maxSize, minSize, newScore / (pointsToWin - 1f)));
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }

    void ChangeTargetingBias() =>
        targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);

    void SetSize(float newSize)
    {
        size = newSize;
        Vector3 s = transform.localScale;
        s.x = 2f * newSize;
        transform.localScale = s;
    }
}
