using UnityEngine;
using System.Collections.Generic;
using System;

public class UiAnimationController : MonoBehaviour
{
    public bool ShowDebug = true;

    List<UiAnimationComponent> Animations = new List<UiAnimationComponent>();
    Queue<IAnimatable> Queue = new Queue<IAnimatable>();

    IAnimatable currentAnim;

    public void AddAction(string name, Action action)
    {
        var anim = new UiAnimationAction(name, action);
        Animations.Add(anim);
    }

    public void AddAnim(UiAnimationComponent anim)
    {
        Debug.LogFormat("Add anim: {0}", anim.Name);
        Animations.Add(anim);
    }

    public void AddDelay(float delay)
    {
        Debug.LogFormat("Add delay: {0}", delay);
        Animations.Add(new UiAnimationDelay(this, delay));
    }

    /// <summary>
    /// Plays animations that have been added in the order they were added.
    /// </summary>
    public void PlayAnimations()
    {
        Queue.Clear();
        Animations.ForEach(x => Queue.Enqueue(x));
        UpdateQueue(Queue.Peek());
    }

    public void ClearQueue()
    {
        Queue.Clear();
        Animations.Clear();
    }

    void UpdateQueue(IAnimatable anim)
    {
        anim.OnDone -= UpdateQueue;
        if (Queue.Count != 0)
        {
            currentAnim = Queue.Dequeue();

            var name = string.IsNullOrEmpty(currentAnim.Name) ? "Unnamed Anim" : currentAnim.Name;

            Debug.LogFormat("Play anim: {0}", name);
            currentAnim.OnDone += UpdateQueue;
            PlayAnim(currentAnim);
        }
        else
        {
            Debug.LogFormat("Finished playing anims!");
            ClearQueue();
        }
    }

    /// <summary>
    /// Plays all animations in the queue.
    /// </summary>
    void PlayAnim(IAnimatable anim)
    {
        anim.Play();
    }
}
