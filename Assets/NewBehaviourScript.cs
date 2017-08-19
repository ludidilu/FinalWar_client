using screenScale;
using superFunction;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Transform container;

    [SerializeField]
    private Transform fixContainer;

    [SerializeField]
    private Camera renderCamera;

    [SerializeField]
    private float viewportXFix;

    [SerializeField]
    private float viewportYFix;

    [SerializeField]
    private float boundFix;

    private Bounds bounds;

    private Bounds viewport;

    // Use this for initialization
    void Start()
    {
        Vector3 v = renderCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        viewport = new Bounds(Vector3.zero, v * 2);

        viewport.center = new Vector3(viewportXFix, viewportYFix, 0);

        viewport.extents = new Vector3(viewport.extents.x - viewportXFix, viewport.extents.y - viewportYFix, viewport.extents.z);

        CreateBoudns();

        SuperFunction.Instance.AddEventListener<float, Vector2>(ScreenScale.Instance.gameObject, ScreenScale.SCALE_CHANGE, ScreenScaleChange);
    }

    private void ScreenScaleChange(int _index, float _scale, Vector2 _pos)
    {
        Vector3 v = renderCamera.ScreenToWorldPoint(_pos);

        Vector3 v2 = container.InverseTransformPoint(v);

        container.localPosition = new Vector3(container.localPosition.x + v2.x, container.localPosition.y + v2.y, container.localPosition.z);

        if (_scale > 1)
        {
            container.localScale = container.localScale * 1.05f;
        }
        else
        {
            container.localScale = container.localScale * (1 / 1.05f);
        }

        Vector3 v3 = container.TransformPoint(v2);

        container.localPosition = new Vector3(container.localPosition.x - v3.x + v.x, container.localPosition.y - v3.y + v.y, container.localPosition.z);

        FixBounds();
    }

    private void CreateBoudns()
    {
        Renderer[] renderers = container.GetComponentsInChildren<Renderer>();

        bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        fixContainer.localPosition = new Vector3(-bounds.center.x, -bounds.center.y, 0);
    }

    private Vector3 lastPos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPos = renderCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 v = renderCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 v2 = v - lastPos;

            container.localPosition = new Vector3(container.localPosition.x + v2.x, container.localPosition.y + v2.y, container.localPosition.z);

            lastPos = v;
        }

        FixBounds();
    }

    private void FixBounds()
    {
        Bounds tmpBounds = bounds;

        tmpBounds.extents = new Vector3(tmpBounds.extents.x * container.transform.localScale.x, tmpBounds.extents.y * container.transform.localScale.y, tmpBounds.extents.z * container.transform.localScale.z);

        tmpBounds.Expand(boundFix);

        if (tmpBounds.extents.x < viewport.extents.x)
        {
            container.localPosition = new Vector3(viewport.center.x, container.localPosition.y, container.localPosition.z);
        }
        else if (container.localPosition.x - tmpBounds.extents.x > viewport.min.x)
        {
            container.localPosition = new Vector3(viewport.min.x + tmpBounds.extents.x, container.localPosition.y, container.localPosition.z);
        }
        else if (container.localPosition.x + tmpBounds.extents.x < viewport.max.x)
        {
            container.localPosition = new Vector3(viewport.max.x - tmpBounds.extents.x, container.localPosition.y, container.localPosition.z);
        }

        if (tmpBounds.extents.y < viewport.extents.y)
        {
            container.localPosition = new Vector3(container.localPosition.x, viewport.center.y, container.localPosition.z);
        }
        else if (container.localPosition.y - tmpBounds.extents.y > viewport.min.y)
        {
            container.localPosition = new Vector3(container.localPosition.x, viewport.min.y + tmpBounds.extents.y, container.localPosition.z);
        }
        else if (container.localPosition.y + tmpBounds.extents.y < viewport.max.y)
        {
            container.localPosition = new Vector3(container.localPosition.x, viewport.max.y - tmpBounds.extents.y, container.localPosition.z);
        }
    }
}
