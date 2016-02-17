using UnityEngine;
using System.Collections;
using System;

public interface IAnimatable
{
    event Action<IAnimatable> OnDone;
    string Name { get; }
    void Play();
}
