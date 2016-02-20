using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiTransition : UiBase
{
    public Text Title;
    public Text Message;

    public float TitleScaleFrom;
    public float TitleScaleDuration;
    public float TimeBetween;
    public float MessageFadeDuration;

    public void Show(string title, string message = "")
    {
        base.Show();

        var anim = gameObject.GetOrAddComponent<UiAnimationController>();
        anim.ClearQueue();

        Title.text = title;
        Message.enabled = !string.IsNullOrEmpty(message);
        Message.text = message;

        anim.AddAnim(new UiAnimationFade(Title.gameObject, MessageFadeDuration, UiAnimationFade.FadeType.In), false);
        anim.AddAnim(new UiAnimationScale(Title.gameObject, Vector3.one * TitleScaleFrom, Vector3.one, TitleScaleDuration));
        anim.AddDelay(TimeBetween);
        anim.AddAnim(new UiAnimationFade(Message.gameObject, MessageFadeDuration, UiAnimationFade.FadeType.In));
        anim.PlayAnimations();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
