using UnityEngine.EventSystems;
using UnityEngine.UI;
using superFunction;
using UnityEngine;

public class ClickText : Text, IPointerDownHandler
{
    private static GameObject _eventGo;

    public static GameObject eventGo
    {
        get
        {
            if (_eventGo == null)
            {
                _eventGo = new GameObject("ClickTextEventGo");
            }

            return _eventGo;
        }
    }

    public const string EVENT_NAME = "clickText";

    public int clickKey;

    public void OnPointerDown(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(eventGo, EVENT_NAME, clickKey);
    }
}
