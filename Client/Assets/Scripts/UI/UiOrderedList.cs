using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UiOrderedList : MonoBehaviour
{
    public float RearrangeTime = 0.5f;
    public AnimationCurve RearrangeCurve;

    LayoutGroup LayoutGroup { get; set; }
    Dictionary<object, MonoBehaviour> Items;

    void Awake()
    {
        Items = new Dictionary<object, MonoBehaviour>();
    }

    public TView AddItem<TData, TView>(TData data, TView view, Action<TData, TView> onAdd = null) where TView : MonoBehaviour
    {
        var instance = UiUtility.AddChild<TView>(gameObject, view, true);

        if (onAdd != null)
            onAdd((TData)data, (TView)instance);

        Items.Add(data, instance);

        return instance;
    }

    IEnumerator Rearrange(List<Transform> rows)
    {
        Debug.LogFormat("Rearrange");

        // Disable layout group to prevent auto-layout
        if (LayoutGroup == null)
            LayoutGroup = GetComponent<LayoutGroup>();

        LayoutGroup.enabled = false;

        // Get position of each 'row' here
        var rowPositions = gameObject.GetChildTransforms().Select(x => x.position).ToList();

        for (int i = 0; i < rows.Count; i++)
        {
            Debug.LogFormat("Move {0} from {1} to {2}", rows[i].name, rows[i].position, i);
        }

        // Store initial positions
        var startPositions = rows.Select(x => x.position).ToList();
            
        float time = 0f;
        while (time < RearrangeTime)
        {
            Transform transform = null;
            for (int i = 0; i < rows.Count; i++)
            {
                transform = rows[i];
                transform.position = Vector3.Lerp(startPositions[i], rowPositions[i], RearrangeCurve.Evaluate(time / RearrangeTime));
            }


            time += Time.deltaTime;

            yield return 0;
        }

        Debug.LogFormat("Finished!");

        // Renable layout group and set correct transform order
        foreach (var row in rows)
        {
            row.SetAsLastSibling();
        }

        LayoutGroup.enabled = true;
    }

    public void OrderBy<TData>(Comparison<TData> comparison)
    {
        var items = Items.ToList();

        items.Sort((a, b) => -comparison((TData)a.Key, (TData)b.Key));

        items.ForEach(x => Debug.LogFormat("New Order: {0}", x.Key));

        var transforms = items.Select(x => x.Value.transform).ToList();

        StartCoroutine(Rearrange(transforms));
    }
}
