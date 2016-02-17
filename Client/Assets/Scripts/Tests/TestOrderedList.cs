using UnityEngine;
using System.Collections.Generic;
using Protocol;

public class TestOrderedList : MonoBehaviour
{
    public UiOrderedList OrderedList;
    public UiResultRow Row;

    List<Mock> mockData;

    public class Mock
    {
        public int Value;

        public Mock(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    void Start()
    {
        Row.gameObject.SetActive(false);

        mockData = new List<Mock>();
        mockData.Add(new Mock(1));
        mockData.Add(new Mock(3));
        mockData.Add(new Mock(5));
        mockData.Add(new Mock(4));
        mockData.Add(new Mock(2));
        mockData.Add(new Mock(6));


        mockData.ForEach(x => OrderedList.AddItem<Mock, UiResultRow>(x, Row, ((data, view) =>
        {
            view.PlayerName.text = data.Value.ToString();
            view.name = data.ToString();
        })));
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            OrderedList.OrderBy<Mock>((a, b) => a.Value.CompareTo(b.Value));
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            OrderedList.OrderBy<Mock>((a, b) => b.Value.CompareTo(a.Value));
        }
    }
}
