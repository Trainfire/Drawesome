using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UiGameStateChoosing : UiGameState
{
    public event Action<UiChoosingItem> OnChoiceSelected;

    public UiChoosingItem ChoicePrototype;
    public List<RectTransform> SpawnPositions = new List<RectTransform>();

    List<UiChoosingItem> Views = new List<UiChoosingItem>();

    void Awake()
    {
        ChoicePrototype.gameObject.SetActive(false);
    }

    public void ShowChoices(List<string> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            Debug.LogFormat("Show choice: {0}", choices[i]);

            var instance = UiUtility.AddChild(SpawnPositions[i], ChoicePrototype, true);

            instance.Text.text = choices[i];

            instance.Button.onClick.AddListener(() =>
            {
                if (OnChoiceSelected != null)
                    OnChoiceSelected(instance);
            });

            Views.Add(instance);
        }
    }

    protected override void OnEnd()
    {
        Views.ForEach(x => Destroy(x.gameObject));
    }
}
