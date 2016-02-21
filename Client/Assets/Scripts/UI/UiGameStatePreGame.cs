using UnityEngine;
using UnityEngine.UI;

public class UiGameStatePreGame : UiBase
{
    public GameObject Top;

    public GameObject InfoBox;
    public Text InfoLabel;
    public Button Start;
    public Button Cancel;

    public Text ReadyTitle;
    public Text ReadyTimer;

    UiAnimationController animController;
    UiAnimationController AnimController
    {
        get
        {
            if (animController == null)
                animController = gameObject.GetOrAddComponent<UiAnimationController>();
            return animController;
        }
    }

    void Awake()
    {
        Setup();
    }

    public void SetCountdown(float duration)
    {
        int ticks = (int)duration;

        AnimController.ClearQueue();

        Setup();

        // Fade out top
        AnimController.AddAnim(new UiAnimationFade(Top.gameObject, 0.25f, UiAnimationFade.FadeType.Out), false);

        // Fade and scale in title simultaneously
        AnimController.AddAnim(new UiAnimationFade(ReadyTitle.gameObject, 0.5f, UiAnimationFade.FadeType.In), false);

        // Add countdowns
        for (int i = 0; i < ticks; i++)
        {
            // Make temp var
            var time = ticks - i;

            AnimController.AddDelay(1f);

            // Set timer text on every tick
            AnimController.AddAction("Set Timer Text", () => ReadyTimer.text = time.ToString());

            // Scale and fade in timer
            AnimController.AddAnim(new UiAnimationFade(ReadyTimer.gameObject, 0.1f, UiAnimationFade.FadeType.In), false);
            AnimController.AddAnim(new UiAnimationScale(ReadyTimer.gameObject, Vector3.one * 2, Vector3.one, 0.1f));
        }

        AnimController.PlayAnimations();
    }

    public void CancelCountdown()
    {
        AnimController.ClearQueue();

        // Fade out elements
        AnimController.AddAnim(new UiAnimationFade(ReadyTitle.gameObject, 0.25f, UiAnimationFade.FadeType.Out), false);
        AnimController.AddAnim(new UiAnimationFade(ReadyTimer.gameObject, 0.25f, UiAnimationFade.FadeType.Out), false);

        // Fade in top
        AnimController.AddAnim(new UiAnimationFade(Top.gameObject, 0.25f, UiAnimationFade.FadeType.In), false);

        AnimController.PlayAnimations();
    }

    /// <summary>
    /// Setup initial alpha values
    /// </summary>
    void Setup()
    {
        // Set initial alpha via hack :^)
        AnimController.AddAnim(new UiAnimationFade(ReadyTitle.gameObject, 0f, UiAnimationFade.FadeType.Out), false);
        AnimController.AddAnim(new UiAnimationFade(ReadyTimer.gameObject, 0f, UiAnimationFade.FadeType.Out), false);

        AnimController.PlayAnimations();
    }
}
