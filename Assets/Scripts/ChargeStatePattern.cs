using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeContext
{
    void SetState(IChargeStatePattern newState);
}
public interface IChargeStatePattern
{
    void StartCharging(IChargeContext context);
    void BounceCharged(IChargeContext context);
    void BounceNormal(IChargeContext context);
    void RunOutOfTime(IChargeContext context);
}

public class ChargeStatePattern : MonoBehaviour, IChargeContext
{
    IChargeStatePattern currentState = new NormalBounceState();

    public void StartCharging() => currentState.StartCharging(this);
    public void BounceCharged() => currentState.BounceCharged(this);
    public void BounceNormal() => currentState.BounceNormal(this);
    public void RunOutOfTime() => currentState.RunOutOfTime(this);

    void IChargeContext.SetState(IChargeStatePattern newState)
    {
        currentState = newState;
    }
}

public class NormalBounceState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) {
        context.SetState(new ChargeState());
    }
    public void BounceCharged(IChargeContext context) { }
    public void BounceNormal(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }

}

public class ChargeState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) { }
    public void BounceCharged(IChargeContext context) {
        context.SetState(new ChargedBounceState());
    }
    public void BounceNormal(IChargeContext context) {
        context.SetState(new NormalBounceState());
    }
    public void RunOutOfTime(IChargeContext context) {
        context.SetState(new NormalBounceState());
    }
}

public class ChargedBounceState : IChargeStatePattern
{
    public void StartCharging(IChargeContext context) { }
    public void BounceCharged(IChargeContext context) { }
    public void BounceNormal(IChargeContext context) { }
    public void RunOutOfTime(IChargeContext context) { }
}
