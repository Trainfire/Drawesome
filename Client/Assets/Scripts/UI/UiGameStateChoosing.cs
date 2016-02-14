using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UiGameStateChoosing : UiBase
{
    public event Action<UiChoosingItem> OnChoiceSelected;

    public UiChoosingItem ChoicePrototype;
    public List<RectTransform> SpawnPositions = new List<RectTransform>();

    List<UiChoosingItem> Views = new List<UiChoosingItem>();

    protected override void OnFirstShow()
    {
        Views = new List<UiChoosingItem>();
    }

    public void ShowChoices(List<string> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            Debug.LogFormat("Show choice: {0}", choices[i]);

            var instance = UiUtility.AddChild(SpawnPositions[i].gameObject, ChoicePrototype, true);

            instance.Text.text = choices[i];

            instance.Button.onClick.AddListener(() =>
            {
                if (OnChoiceSelected != null)
                    OnChoiceSelected(instance);
            });

            Views.Add(instance);
        }
    }

    protected override void OnHide()
    {
        Views.ForEach(x => Destroy(x.gameObject));
        Views.Clear();
    }
}
