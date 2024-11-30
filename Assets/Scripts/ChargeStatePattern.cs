using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public interface IChargeContext
{
    void SetState(IChargeStatePattern newState);
}
public interface IChargeStatePattern
{
    void StartCharging(IChargeContext context);
    void StopCharging(IChargeContext context);
    void StartPowerTime(IChargeContext context);
    void EndPowerTime(IChargeContext context);
    void RunOutOfTime(IChargeContext context);
    void PerformAChargedBounce(IChargeContext context);
    void EnterState(PaddleWithState paddle);
    void UpdateState(IChargeContext context);
}

public class ChargeStatePattern : MonoBehaviour, IChargeContext
{
    PaddleWithState paddle;
    IChargeStatePattern currentState;
    void Awake()
    {
        //paddle = (PaddleWithState)this.gameObject;
        paddle = GetComponent<PaddleWithState>();
        currentState = new NormalBounceState(paddle.getSize());
        //Debug.Log("paddle", paddle);
        //Debug.Log("paddle size: " + (paddle.getSize()));
    }

    void Update()
    {
        //Debug.Log("Updating current state: " + currentState.GetType().Name);
        currentState.UpdateState(this);
    }

    public void StartCharging() => currentState.StartCharging(this);
    public void StopCharging() => currentState.StopCharging(this);
    public void StartPowerTime() => currentState.StartPowerTime(this);
    public void EndPowerTime() => currentState.EndPowerTime(this);
    public void RunOutOfTime() => currentState.RunOutOfTime(this);
    public void PerformAChargedBounce() => currentState.PerformAChargedBounce(this);
    public void EnterState(PaddleWithState paddle) => currentState.EnterState(paddle);
    public void UpdateState() => currentState.UpdateState(this);


    void IChargeContext.SetState(IChargeStatePattern newState)
    {
        currentState = newState;
        currentState.EnterState(paddle);
    }
}

public class NormalBounceState : IChargeStatePattern
{
    float originalSize;
    PaddleWithState paddle;
    public NormalBounceState(float originalSize)
    {
        this.originalSize = originalSize;
    }

    public void StartCharging(IChargeContext context) {
        Debug.Log("StartCharging in normal");
        context.SetState(new ChargeTransitionState(originalSize, 0));
    }
    public void StopCharging(IChargeContext context) {
        Debug.Log("StopCharging normal");
    }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) { }
    public void EnterState(PaddleWithState paddle) {
        Debug.Log("Normalno stanje");
        this.paddle = paddle;
    }
    public void UpdateState(IChargeContext context) { }
}

public class ChargeTransitionState : IChargeStatePattern
{
    PaddleWithState paddle;
    float[] chargeSizes = new float[3];
    float[] chargeSizeChanges = { 1f, 0.9f, 0.6f };
    float[] resizeDurations = { 0.5f, 0.3f, 0.2f };

    float chargeTimer;
    float originalSize;
    float currentSize;

    int chargeLevel;
    public ChargeTransitionState(float originalSize, int chargeLevel)
    {
        this.originalSize = originalSize;
        this.chargeLevel = chargeLevel;
    }
    public void StartCharging(IChargeContext context) {
        Debug.Log("StartCharging in transition");
    }
    public void StopCharging(IChargeContext context) {
        Debug.Log("StopCharging in transition");
        context.SetState(new NormalBounceState(originalSize));
    }
    public void StartPowerTime(IChargeContext context) {
        context.SetState(new PowerfulChargeState(originalSize, chargeLevel));
    }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) { }
    public void EnterState(PaddleWithState paddle) {
        this.paddle = paddle;
        //Debug.Log("originalSize: "+ originalSize);
        chargeTimer = 0f;
        chargeSizes = getSizes();
        //Debug.Log("chargeLevel: "+ chargeLevel);
        Debug.Log("Evo me v transition");
    }
    public void UpdateState(IChargeContext context)
    {
        /*
        Debug.Log("chargeLevel");
        Debug.Log(chargeLevel);
        Debug.Log("chargeTimer");
        Debug.Log(chargeTimer);
        Debug.Log("chargeSizes");
        foreach (var charge in chargeSizes)
        {
            Debug.Log(charge);
        }
        */
        
        if (chargeLevel < 3)
        {
            float currentResizeDuration = resizeDurations[chargeLevel];
            chargeTimer = chargeTimer < currentResizeDuration ? chargeTimer + Time.deltaTime : currentResizeDuration;
            currentSize = chargeLevel > 0 ? chargeSizes[chargeLevel - 1] : originalSize;
            float nextSize = chargeSizes[chargeLevel];
            //Debug.Log("nextSize: " + nextSize);

            // dokler traja nivo charge-anja manjšam paddle
            if (chargeTimer < currentResizeDuration)
            {
                paddle.SetSize(Mathf.Lerp(currentSize, nextSize, chargeTimer / currentResizeDuration));
            }
            else // Med vsakim nivojem obstaja èas ko paddle udari žogo z veèjo moèjo
            {
               StartPowerTime(context);
            }
        }
    }

    float[] getSizes()
    {
        //Debug.Log("currentSize: "+ originalSize);
        float[] chargeSizes = new float[3];
        for (int i = 0; i < chargeSizeChanges.Length; i++)
        {
            chargeSizes[i] = i == 0 ? originalSize - chargeSizeChanges[i] : chargeSizes[i - 1] - chargeSizeChanges[i];
            //Debug.Log(chargeSizes[i]);
        }
        return chargeSizes;
    }

    /*
void PrepareForCharging() {
    float[] chargeSizes = getSizes();
}

void ChargePaddle(int chargeLevel, float chargeTimer)
{
    if (chargeLevel < 3)
    {
        float[] chargeSizes = getSizes();
        float currentResizeDuration = resizeDurations[chargeLevel];
        chargeTimer = chargeTimer < currentResizeDuration ? chargeTimer + Time.deltaTime : currentResizeDuration;
        currentSize = chargeLevel > 0 ? chargeSizes[chargeLevel - 1] : currentSize;
        float nextSize = chargeSizes[chargeLevel];
        Debug.Log("currentResizeDuration");
        Debug.Log(currentResizeDuration);
        Debug.Log("currentSize");
        Debug.Log(currentSize);

    }

    if (chargeLevel < 3)
    {
        float currentResizeDuration = resizeDurations[chargeLevel];
        chargeTimer = chargeTimer < currentResizeDuration ? chargeTimer + Time.deltaTime : currentResizeDuration;
        currentSize = chargeLevel > 0 ? chargeSizes[chargeLevel - 1] : currentSize;
        float nextSize = chargeSizes[chargeLevel];

        if (chargeTimer >= currentResizeDuration)
        {
            waitBetweenCharges -= Time.deltaTime;
            // med vsakim nivojem charge-a je kratek èasovni interval, ko se ob zadetku žogica pohitri
            if (waitBetweenCharges <= 0f)
            {
                waitBetweenCharges = 0.5f;
                chargeTimer = 0f;
                chargeLevel++;
                chargeMultiplier = 1f;
            }
            else
            {
                // tle pospešimo žogco
                chargeMultiplier = 1f + ((chargeLevel + 1) * 0.25f);
            }
        }
        else
        {
            SetSize(Mathf.Lerp(currentSize, nextSize, chargeTimer / currentResizeDuration));
        }
    }
    else
    {
        chargeOvertime += Time.deltaTime;
        chargeInterrupt = chargeOvertime >= 0.5f ? true : false;
        chargeMultiplier = 2f;
    }

}
*/
}

public class PowerfulChargeState : IChargeStatePattern
{
    PaddleWithState paddle;
    float powerfulStateTimer = 0.5f;
    float originalSize;
    int chargelevel;

    public PowerfulChargeState(float originalSize, int chargeLevel)
    {
        this.originalSize = originalSize;
        this.chargelevel = chargeLevel;
    }

    public void StartCharging(IChargeContext context) { }
    public void StopCharging(IChargeContext context) {
        Debug.Log("StopCharging PowerfulCharge");
        context.SetState(new ChargedBounceState());
    }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context)
    {
        chargelevel++;
        context.SetState(new ChargeTransitionState(originalSize, chargelevel));
    }
    public void RunOutOfTime(IChargeContext context)
    {
        Debug.Log("Teci ven iz èasa");
        paddle.SetSize(originalSize);
        context.SetState(new NormalBounceState(originalSize));
    }
    public void PerformAChargedBounce(IChargeContext context) { }
    public void EnterState(PaddleWithState paddle) {
        Debug.Log("Epsko stanje");
        this.paddle = paddle;
        //Debug.Log("paddle current size: "+ originalSize);
    }
    public void UpdateState(IChargeContext context) {
        //Debug.Log("Sm v updejti");
        if (powerfulStateTimer > 0f)
        {
            powerfulStateTimer -= Time.deltaTime;
            //Debug.Log("powerfulStateTimer: "+ powerfulStateTimer);
        } else
        {
            //Debug.Log("èarž level: " + chargelevel);
            if (chargelevel == 2) {
                // ko je konèana zadnja stopnja charge-anja povrni paddle v svojo originalo velikost
                RunOutOfTime(context);
            } else
            {
                // back to transition charge state
                EndPowerTime(context);
            }
        }
    }
}

public class ChargedBounceState : IChargeStatePattern
{
    PaddleWithState paddle;
    public void StartCharging(IChargeContext context) { }
    public void StopCharging(IChargeContext context) {
        Debug.Log("StopCharging ChargedBounce");
    }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) {
        context.SetState(new NormalBounceState(5f));
    }
    public void EnterState(PaddleWithState paddle) {
        Debug.Log("Napeto stanje");
        this.paddle = paddle;
    }
    public void UpdateState(IChargeContext context) {
        //Debug.Log("I am charged");
        // charged bounce
        PerformAChargedBounce(context);
    }
}

// used to disable charging when still pressing space after the timer has run out
/*
public class AfterChargeState : IChargeStatePattern
{
    PaddleWithState paddle;
    float originalSize;

    public AfterChargeState(float originalSize)
    {
        this.originalSize = originalSize;
    }
    public void StartCharging(IChargeContext context) {
        Debug.Log("StartCharging AfterChargeState");
    }
    public void StopCharging(IChargeContext context) {
        Debug.Log("StopCharging AfterChargeState");
        context.SetState(new NormalBounceState(originalSize));
    }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context){ }
    public void EnterState(PaddleWithState paddle)
    {
        Debug.Log("Po charge-i stanje");
        this.paddle = paddle;
    }
    public void UpdateState(IChargeContext context) { }
}
*/
