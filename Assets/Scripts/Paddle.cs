using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    int chargePhases = 3;

    [SerializeField, Min(0f)]
    float
        minSize = 2f,
        maxSize = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f,
        chargeDuration = 1f,
        chargeResize = 2.5f;

    float
        size,
        targetingBias,
        minChargeSize = 0,
        currentSize = 0,
        chargeTimer,
        chargeOvertime,
        waitBetweenCharges = 0f,
        chargeMultiplier = 1f;

    int 
        chargeLevel = 0,
        score;

    bool charge = false,
        chargeInterrupt = false,
        endRoundCharge = false;

    float[] chargeSizes = new float[3];
    float[] chargeSizeChanges = { 1f, 0.9f, 0.6f };
    float[] resizeDurations = { 0.5f, 0.3f, 0.2f };

    [SerializeField]
    TextMeshPro scoreText;

    [SerializeField]
    MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    KeyCode
        goRightKey,
        goLeftKey,
        chargeKey;

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
        bool success = -1f <= hitFactor && hitFactor <= 1f;
        if (success)
        {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }
        return success;
    }

    void SetScore(int newScore, float pointsToWin = 10f)
    {
        score = newScore == -1 ? 0 : newScore;
        scoreText.SetText("{0}", score);
        scoreMaterial.SetColor(faceColorId, goalColor * (score / pointsToWin));
        if (newScore != -1)
        {
            Debug.Log("Nje se zdej zajebavat");
            endRoundCharge = true;
            SetSize(Mathf.Lerp(maxSize, minSize, score / (pointsToWin - 1f)));
        }
    }

    public void StartNewGame()
    {
        SetScore(-1);
        ChangeTargetingBias();
    }

    public void StartNewRound()
    {
        ChangeTargetingBias();
    }

    public void ResetPaddleSizes()
    {

        endRoundCharge = true;
        SetSize(Mathf.Lerp(maxSize, minSize, 0));
    }

    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }

    void ChangeTargetingBias() =>
        targetingBias = UnityEngine.Random.Range(-maxTargetingBias, maxTargetingBias);

    void SetSize(float newSize)
    {
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

    public void HandleCharging()
    {
        bool chargeStart = Input.GetKeyDown(chargeKey);
        bool chargeFinish = Input.GetKeyUp(chargeKey);

        if (endRoundCharge)
        {
            chargeInterrupt = false;
            currentSize = size;
            charge = false;
        } else
        {
            charge = chargeInterrupt == false ? Input.GetKey(chargeKey) : false;
        }

        if (chargeStart)
        {
            chargeTimer = 0f;
            minChargeSize = size - chargeResize;
            chargeLevel = 0;
            chargeOvertime = 0f;
            for (int i = 0; i < chargeSizeChanges.Length; i++)
            {
                chargeSizes[i] = i == 0 ? size - chargeSizeChanges[i] : chargeSizes[i - 1] - chargeSizeChanges[i];
            }
            waitBetweenCharges = 0.5f;
            endRoundCharge = false;
            chargeMultiplier = 1f;
        }
        else if (chargeFinish)
        {
            SetSize(currentSize);
            chargeInterrupt = false;
            chargeMultiplier = 1f;
            //endRoundCharge = false;
        }
        else if (chargeInterrupt) // ko pote�e �as charganja, ampak igralec �e dr�i charge gumb
        {
            SetSize(currentSize);
        }
        else if (charge)
        {
            ChargePaddle(currentSize);
        }
        
    }

    void ChargePaddle(float currentSize)
    {
        if (chargeLevel < 3)
        {
            float currentResizeDuration = resizeDurations[chargeLevel];
            chargeTimer = chargeTimer < currentResizeDuration ? chargeTimer + Time.deltaTime : currentResizeDuration;
            currentSize = chargeLevel > 0 ? chargeSizes[chargeLevel - 1] : currentSize;
            float nextSize =  chargeSizes[chargeLevel];

            if (chargeTimer >= currentResizeDuration)
            {
                waitBetweenCharges -= Time.deltaTime;
                // med vsakim nivojem charge-a je kratek �asovni interval, ko se ob zadetku �ogica pohitri
                if (waitBetweenCharges <= 0f)
                {
                    waitBetweenCharges = 0.5f;
                    chargeTimer = 0f;
                    chargeLevel++;
                    chargeMultiplier = 1f;
                } else
                {
                    // tle pospe�imo �ogco
                    chargeMultiplier = 1f + ((chargeLevel + 1) * 0.25f);
                }
            }
            else
            {
                SetSize(Mathf.Lerp(currentSize, nextSize, chargeTimer / currentResizeDuration));
            }
        } else
        {
            chargeOvertime += Time.deltaTime;
            chargeInterrupt = chargeOvertime >= 0.5f ? true : false;
            chargeMultiplier = 2f;
        }
    }

    public float GetChargeMultiplier()
    {
        return chargeMultiplier;
    }

    public void SetChargeMultiplier(float chargeMultiplier)
    {
       this.chargeMultiplier = chargeMultiplier;
    }
}
