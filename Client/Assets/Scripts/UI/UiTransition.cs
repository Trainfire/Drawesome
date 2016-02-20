using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiTransition : UiBase
{
    public Text Title;
    public Text Message;

    public void Show(string title, string message = "")
    {
        base.Show();

        var anim = gameObject.GetOrAddComponent<UiAnimationController>();
        anim.ClearQueue();

        Title.text = title;
        Message.enabled = !string.IsNullOrEmpty(message);
        Message.text = message;

        anim.AddAnim(new UiAnimationScale(gameObject, Vector3.one * 5, Vector3.one, 0.1f));
        anim.PlayAnimations();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
