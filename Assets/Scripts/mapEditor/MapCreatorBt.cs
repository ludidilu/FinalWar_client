using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreatorBt : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int index;

    public void OnPointerClick(PointerEventData _data)
    {
        SendMessageUpwards("BtClick", index, SendMessageOptions.DontRequireReceiver);
    }
}
