using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeContext
{
    void SetState(IChargeStatePattern newState);
}
public interface IChargeStatePattern
{
    void StartCharging(IChargeContext Context);
    void BounceCharged(IChargeContext Context);
    void BounceNormal(IChargeContext Context);
    void RunOutOfTime(IChargeContext Context);
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
    public void StartCharging(IChargeContext Context) { }
    public void BounceCharged(IChargeContext Context) { }
    public void BounceNormal(IChargeContext Context) { }
    public void RunOutOfTime(IChargeContext Context) { }

}

public class ChargeState : IChargeStatePattern
{
    public void StartCharging(IChargeContext Context) { }
    public void BounceCharged(IChargeContext Context) { }
    public void BounceNormal(IChargeContext Context) { }
    public void RunOutOfTime(IChargeContext Context) { }
}

public class ChargedBounceState : IChargeStatePattern
{
    public void StartCharging(IChargeContext Context) { }
    public void BounceCharged(IChargeContext Context) { }
    public void BounceNormal(IChargeContext Context) { }
    public void RunOutOfTime(IChargeContext Context) { }
}
