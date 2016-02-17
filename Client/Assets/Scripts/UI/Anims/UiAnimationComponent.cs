using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class UiAnimationComponent : IAnimatable
{
    public event Action<IAnimatable> OnDone;

    public GameObject Target { get; private set; }
    public bool WaitForCompletion;
    public bool Playing { get; protected set; }
    public virtual string Name { get; set; }

    public virtual void Setup()
    {

    }

    public virtual void Play()
    {
        
    }

    protected virtual void Done()
    {
        if (OnDone != null)
            OnDone(this);
    }

    public UiAnimationComponent()
    {
        
    }

    public UiAnimationComponent(GameObject target, bool waitForCompletion = true) : this()
    {
        Target = target;
        WaitForCompletion = waitForCompletion;
    }

    public bool HasTarget
    {
        get
        {
            return Target != null;
        }
    }
}
