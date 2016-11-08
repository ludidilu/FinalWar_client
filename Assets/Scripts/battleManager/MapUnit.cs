using UnityEngine;
using xy3d.tstd.lib.superFunction;
using xy3d.tstd.lib.superRaycast;

public class MapUnit : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer mainMr;

    private MeshRenderer mr;

    public int index { private set; get; }

    private int index2;

    public void Init(int _index, int _index2, MeshRenderer _mr)
    {
        index = _index;
        index2 = _index2;

        mr = _mr;
    }

    // Use this for initialization
    void Awake()
    {
        SuperFunction.Instance.AddEventListener(gameObject, SuperRaycast.GetMouseButtonDown, GetMouseDown);

        //		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseButtonUp, GetMouseUp);

        SuperFunction.Instance.AddEventListener(gameObject, SuperRaycast.GetMouseEnter, GetMouseEnter);

        SuperFunction.Instance.AddEventListener(gameObject, SuperRaycast.GetMouseExit, GetMouseExit);

        //		SuperFunction.Instance.AddEventListener (gameObject, SuperRaycast.GetMouseClick, GetMouseClick);
    }

    public void SetMainColor(Color _color)
    {
        mainMr.material.SetColor("_Color", _color);

        if (mr != null)
        {
            mr.material.SetVector("colors" + index2, _color);
        }
    }

    private void GetMouseDown(SuperEvent e)
    {
        SendMessageUpwards("MapUnitDown", this, SendMessageOptions.DontRequireReceiver);
    }

    //	private void GetMouseUp(SuperEvent e){
    //
    //		SendMessageUpwards ("MapUnitUp", this, SendMessageOptions.DontRequireReceiver);
    //	}

    private void GetMouseEnter(SuperEvent e)
    {
        SendMessageUpwards("MapUnitEnter", this, SendMessageOptions.DontRequireReceiver);
    }

    private void GetMouseExit(SuperEvent e)
    {
        SendMessageUpwards("MapUnitExit", this, SendMessageOptions.DontRequireReceiver);
    }

    private void GetMouseClick(SuperEvent e)
    {
        SendMessageUpwards("MapUnitUpAsButton", this, SendMessageOptions.DontRequireReceiver);
    }
}
