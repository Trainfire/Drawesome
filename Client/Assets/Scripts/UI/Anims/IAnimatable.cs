using UnityEngine;
using System.Collections;
using System;

public interface IAnimatable
{
    event Action<IAnimatable> OnDone;
    bool WaitForCompletion { get; set; }
    string Name { get; }
    void Play();
}
