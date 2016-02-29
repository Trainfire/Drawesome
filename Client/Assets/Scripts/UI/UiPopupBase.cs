using UnityEngine;
using UnityEngine.UI;

public abstract class UiPopupBase : UiBase
{   
    protected override void OnShow()
    {
        AnimController.ClearQueue();
        AnimController.AddAnim(new UiAnimationFade(gameObject, 0.25f, UiAnimationFade.FadeType.In));
        AnimController.PlayAnimations();
    }

    public override void Hide()
    {
        AnimController.ClearQueue();
        AnimController.AddAnim(new UiAnimationFade(gameObject, 0.25f, UiAnimationFade.FadeType.Out));
        AnimController.AddDelay(0.5f);
        AnimController.AddAction("Destroy", () => Destroy(gameObject));
        AnimController.PlayAnimations();
    }
}
