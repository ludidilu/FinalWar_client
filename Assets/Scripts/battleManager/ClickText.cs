using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using superFunction;

public class ClickText : MonoBehaviour, IPointerClickHandler
{
    public const string EVENT_NAME = "clickText";

    [SerializeField]
    private Text text;

    public void OnPointerClick(PointerEventData _data)
    {
        SuperFunction.Instance.DispatchEvent(BattleManager.Instance.gameObject, EVENT_NAME, text.text);
    }
}
