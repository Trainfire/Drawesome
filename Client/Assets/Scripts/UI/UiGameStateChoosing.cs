using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;

public class UiGameStateChoosing : UiBase
{
    public event Action<AnswerData> OnChoiceSelected;
    public event Action<AnswerData> OnLike;

    public UiChoice ChoicePrototype;
    public GameObject Left;
    public GameObject Right;
    public UiInfoBox InfoBox;

    List<UiChoice> Views = new List<UiChoice>();

    protected override void OnFirstShow()
    {
        Views = new List<UiChoice>();
        ChoicePrototype.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        base.OnShow();
        InfoBox.Hide();
    }

    public void ShowChoices(PlayerData creator, List<AnswerData> choices)
    {
        if (Client.IsPlayer(creator))
            InfoBox.Show(Strings.PlayersOwnDrawing);

        for (int i = 0; i < choices.Count - 1; i++)
        {
            Debug.LogFormat("Show choice: {0}", choices[i]);

            var position = i % 2 == 1 ? Left : Right;
            var instance = UiUtility.AddChild(position.gameObject, ChoicePrototype, true);

            // Show different UI depending of if this is the player's answer
            // If it is, the player won't be able to select their own answer
            bool isPlayer = choices[i].Author.ID == Client.PlayerData.ID;
            bool isCreator = Client.IsPlayer(creator);

            instance.YourAnswer.SetActive(isPlayer);
            instance.OtherPlayersAnswer.SetActive(!isPlayer);
            instance.Text.ForEach(x => x.text = choices[i].Answer);

            // Don't allow button presses if the drawing creator is the player
            instance.Button.interactable = !isPlayer && !isCreator;
            instance.Like.interactable = !isPlayer;

            if (!isPlayer && !isCreator)
            {
                var tempChoice = choices[i];

                instance.Button.onClick.AddListener(() =>
                {
                    if (OnChoiceSelected != null)
                        OnChoiceSelected(tempChoice);

                    DisableButtons();

                    InfoBox.Show(Strings.ChoiceSubmitted);
                });

                instance.Like.onClick.AddListener(() =>
                {
                    if (OnLike != null)
                        OnLike(tempChoice);
                });
            }

            Views.Add(instance);
        }
    }

    public void DisableButtons()
    {
        Views.ForEach(x => x.Button.interactable = false);
    }

    protected override void OnHide()
    {
        Views.ForEach(x => Destroy(x.gameObject));
        Views.Clear();
    }
}
