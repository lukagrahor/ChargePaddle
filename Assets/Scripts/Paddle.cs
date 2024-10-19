using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        minSize = 2f,
        maxSize = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f,
        chargeDuration = 3f,
        chargeResize = 2.5f;

    [SerializeField]
    TextMeshPro scoreText;

    [SerializeField]
    MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    int score;
    float size, targetingBias;
    KeyCode
        goRightKey,
        goLeftKey,
        chargeKey;

    float chargeTimer;
    int chargePhases = 3;

    float minChargeSize = 0;
    float currentSize = 0;
   
    static readonly int
        emissionColorId = Shader.PropertyToID("_EmissionColor"),
        faceColorId = Shader.PropertyToID("_FaceColor"),
        timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");
    Material goalMaterial, paddleMaterial, scoreMaterial;

    enum Players
    {
        Player1, Player2, AI
    }

    [SerializeField]
    Players player;

    void Awake()
    {
        SetKeys();
        goalMaterial = goalRenderer.material;
        goalMaterial.SetColor(emissionColorId, goalColor);
        paddleMaterial = GetComponent<MeshRenderer>().material;
        scoreMaterial = scoreText.fontMaterial;
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
        bool goRight = Input.GetKey(goRightKey);
        bool goLeft = Input.GetKey(goLeftKey);
        bool charge = Input.GetKey(chargeKey);

        bool chargeStart = Input.GetKeyDown(chargeKey);
        bool chargeFinish = Input.GetKeyUp(chargeKey);

        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        if (chargeStart) {
            chargeTimer = 0f;
            minChargeSize = size - chargeResize;
            currentSize = size;
        }
        else if (chargeFinish) {
            Debug.Log("Stop charging");
        }
        else if (charge)
        {
            chargePaddle(minChargeSize, currentSize);
        }
        return x;
    }

    public bool HitBall(float ballX, float ballSize, out float hitFactor)
    {
        ChangeTargetingBias();
        hitFactor = (ballX - transform.localPosition.x) /
            (size + ballSize);
        bool success = -1f <= hitFactor && hitFactor <= 1f;
        if (success)
        {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }
        return success;
    }

    void SetScore(int newScore, float pointsToWin = 10f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        scoreMaterial.SetColor(faceColorId, goalColor * (newScore / pointsToWin));
        SetSize(Mathf.Lerp(maxSize, minSize, newScore / (pointsToWin - 1f)));
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }

    void ChangeTargetingBias() =>
        targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);

    void SetSize(float newSize)
    {
        Debug.Log(newSize);
        size = newSize;
        Vector3 s = transform.localScale;
        s.x = 2f * newSize;
        transform.localScale = s;
    }

    void SetKeys()
    {
        goRightKey = player == Players.Player1 ? KeyCode.RightArrow : KeyCode.D;
        goLeftKey = player == Players.Player1 ? KeyCode.LeftArrow : KeyCode.A;
        chargeKey = player == Players.Player1 ? KeyCode.Return : KeyCode.Space;
    }

    void chargePaddle(float minChargeSize, float currentSize)
    {
        chargeTimer = chargeTimer < chargeDuration ? chargeTimer + Time.deltaTime : chargeDuration;

        /*
        float chargeOffset = Mathf.Round((chargeDuration / (float)chargePhases) * 10f) / 10f;
        Debug.Log("chargeOffset " + chargeOffset);
        SetSize(size - (chargeTimer * chargeOffset));
        */
 
        Debug.Log("size "+ currentSize);
        Debug.Log("minChargeSize " + minChargeSize);
        Debug.Log("chargeDuration " + chargeDuration);
        Debug.Log("chargeTimer floor " + Mathf.Floor(chargeTimer));

        float test = Mathf.Lerp(currentSize, minChargeSize, Mathf.Ceil(chargeTimer) / chargeDuration);

        Debug.Log("deljenje: " + Mathf.Ceil(chargeTimer) / chargeDuration);
        Debug.Log("Lerpanje: " + test);

        SetSize(Mathf.Lerp(currentSize, minChargeSize, Mathf.Ceil(chargeTimer) / chargeDuration));
        /*
        Debug.Log("Lerp:");
        Debug.Log(Mathf.Lerp(currentSize, minChargeSize, chargeDuration / Mathf.Floor(chargeTimer)));
        Debug.Log("Charging... "+ chargeTimer);
        */
    }
}
