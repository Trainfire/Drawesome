using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UiAnimationController : MonoBehaviour
{
    public bool ShowDebug = true;

    List<UiAnimationComponent> Animations = new List<UiAnimationComponent>();
    Queue<UiAnimationComponent> Queue = new Queue<UiAnimationComponent>();

    UiAnimationComponent currentAnim;
    bool waitingForAnim = false;

    public void AddAnim(UiAnimationComponent anim)
    {
        Debug.LogFormat("Add anim: {0}", anim.GetType());
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

    void UpdateQueue(UiAnimationComponent anim)
    {
        anim.OnDone -= UpdateQueue;
        if (Queue.Count != 0)
        {
            currentAnim = Queue.Dequeue();
            Debug.LogFormat("Play anim: {0}", currentAnim.GetType());
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
    void PlayAnim(UiAnimationComponent anim)
    {
        anim.Play();
    }
}
