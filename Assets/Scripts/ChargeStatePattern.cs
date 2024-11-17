using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeContext
{
    void SetState(IChargeStatePattern newState);
    PaddleWithState Paddle { get; }
}
public interface IChargeStatePattern
{
    void StartCharging(IChargeContext context);
    void StopCharging(IChargeContext context);
    void StartPowerTime(IChargeContext context);
    void EndPowerTime(IChargeContext context);
    void RunOutOfTime(IChargeContext context);
    void PerformAChargedBounce(IChargeContext context);
}

public class ChargeStatePattern : MonoBehaviour, IChargeContext
{
    IChargeStatePattern currentState = new NormalBounceState();
    PaddleWithState paddle;
    void Awake()
    {
        //paddle = (PaddleWithState)this.gameObject;
        paddle = GetComponent<PaddleWithState>();
        Debug.Log("paddle", paddle);
    }

    public PaddleWithState Paddle => paddle;

    public void StartCharging() => currentState.StartCharging(this);
    public void StopCharging() => currentState.StartCharging(this);
    public void StartPowerTime() => currentState.StartPowerTime(this);
    public void EndPowerTime() => currentState.EndPowerTime(this);
    public void RunOutOfTime() => currentState.RunOutOfTime(this);
    public void PerformAChargedBounce() => currentState.PerformAChargedBounce(this);

    void IChargeContext.SetState(IChargeStatePattern newState)
    {
        currentState = newState;
    }
}

public class NormalBounceState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) {
        ChargePaddle(1);
        context.SetState(new ChargeTransitionState());
    }
    public void StopCharging(IChargeContext context) { }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) { }

    int chargeLevel = 0;
    float[] chargeSizes = new float[3];
    float[] chargeSizeChanges = { 1f, 0.9f, 0.6f };
    float[] resizeDurations = { 0.5f, 0.3f, 0.2f };
    float chargeTimer = 0f;

    void ChargePaddle(float currentSize)
    {
        if (chargeLevel < 3)
        {
            float currentResizeDuration = resizeDurations[chargeLevel];
        }
        /*
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
        */
    }

}

public class ChargeTransitionState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) { }
    public void StopCharging(IChargeContext context) {
        context.SetState(new NormalBounceState());
    }
    public void StartPowerTime(IChargeContext context) {
        context.SetState(new PowerfulChargeState());
    }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) { }
}

public class PowerfulChargeState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) { }
    public void StopCharging(IChargeContext context) {
        context.SetState(new ChargedBounceState());
    }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context)
    {
        context.SetState(new ChargeTransitionState());
    }
    public void RunOutOfTime(IChargeContext context)
    {
        context.SetState(new NormalBounceState());
    }
    public void PerformAChargedBounce(IChargeContext context) { }
}

public class ChargedBounceState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) { }
    public void StopCharging(IChargeContext context) { }
    public void StartPowerTime(IChargeContext context) { }
    public void EndPowerTime(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
    public void PerformAChargedBounce(IChargeContext context) {
        context.SetState(new NormalBounceState());
    }
}
