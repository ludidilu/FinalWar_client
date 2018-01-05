using UnityEngine.EventSystems;
using UnityEngine.UI;
using superFunction;
using UnityEngine;

public class ClickImage : Image, IPointerClickHandler
{
    private static GameObject _eventGo;

    public static GameObject eventGo
    {
        get
        {
            if (_eventGo == null)
            {
                _eventGo = new GameObject("ClickImageEventGo");
            }

            return _eventGo;
        }
    }

    public const string EVENT_NAME = "clickImage";

    public int clickKey;

    public void OnPointerClick(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(eventGo, EVENT_NAME, clickKey);
    }
}
