using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Protocol;

public class UiGameStateChoosing : UiBase
{
    public event Action<UiChoosingItem> OnChoiceSelected;

    public UiChoosingItem ChoicePrototype;
    public List<RectTransform> SpawnPositions = new List<RectTransform>();
    public UiInfoBox InfoBox;

    List<UiChoosingItem> Views = new List<UiChoosingItem>();

    protected override void OnFirstShow()
    {
        Views = new List<UiChoosingItem>();
    }

    public void ShowChoices(PlayerData creator, List<AnswerData> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            Debug.LogFormat("Show choice: {0}", choices[i]);

            var instance = UiUtility.AddChild(SpawnPositions[i].gameObject, ChoicePrototype, true);

            // Show different UI depending of if this is the player's answer
            // If it is, the player won't be able to select their own answer
            bool isPlayer = choices[i].Author.ID == Client.PlayerData.ID;
            bool isCreator = Client.IsPlayer(creator);

            instance.YourAnswer.SetActive(isPlayer);
            instance.OtherPlayersAnswer.SetActive(!isPlayer);
            instance.Text.ForEach(x => x.text = choices[i].Answer);

            // Don't allow button presses if the drawing creator is the player
            instance.Button.interactable = !isPlayer && !isCreator;

            if (!isPlayer && !isCreator)
            {
                instance.Button.onClick.AddListener(() =>
                {
                    if (OnChoiceSelected != null)
                        OnChoiceSelected(instance);
                });
            }

            Views.Add(instance);
        }
    }

    protected override void OnHide()
    {
        Views.ForEach(x => Destroy(x.gameObject));
        Views.Clear();
    }
}
