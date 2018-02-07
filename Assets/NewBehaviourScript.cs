using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField]
    private Renderer go;

    [SerializeField]
    private Camera renderCamera;

    [SerializeField]
    private AnimationCurve curve;

    private Vector2 downPos;

    private bool hasDown;

    private const float vv = 0.5f;

    private Vector2 stepV;

    private Bounds viewport;

    private void Awake()
    {
        stepV = new Vector2(renderCamera.aspect * renderCamera.orthographicSize, renderCamera.orthographicSize);

        viewport = new Bounds(Vector3.zero, stepV * 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hasDown = true;

            downPos = renderCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            hasDown = false;
        }
        else if (hasDown)
        {
            Vector2 nowPos = renderCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 v = nowPos - downPos;

            downPos = nowPos;

            if (go.bounds.min.y < viewport.min.y && v.y < 0)
            {
                float dis = viewport.min.y - go.bounds.min.y;

                float scale = Get(dis);

                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + v.y * scale, go.transform.position.z);
            }
            else if (go.bounds.max.y > viewport.max.y && v.y > 0)
            {
                float dis = go.bounds.max.y - viewport.max.y;

                float scale = Get(dis);

                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + v.y * scale, go.transform.position.z);
            }
            else
            {
                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + v.y, go.transform.position.z);
            }

            if (go.bounds.min.x < viewport.min.x && v.x < 0)
            {
                float dis = viewport.min.x - go.bounds.min.x;

                float scale = Get(dis);

                go.transform.position = new Vector3(go.transform.position.x + v.x * scale, go.transform.position.y, go.transform.position.z);
            }
            else if (go.bounds.max.x > viewport.max.x && v.x > 0)
            {
                float dis = go.bounds.max.x - viewport.max.x;

                float scale = Get(dis);

                go.transform.position = new Vector3(go.transform.position.x + v.x * scale, go.transform.position.y, go.transform.position.z);
            }
            else
            {
                go.transform.position = new Vector3(go.transform.position.x + v.x, go.transform.position.y, go.transform.position.z);
            }
        }
        else
        {
            if (go.bounds.min.y < viewport.min.y)
            {
                float dis = viewport.min.y - go.bounds.min.y;

                dis = dis * 0.2f;

                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + dis, go.transform.position.z);
            }
            else if (go.bounds.max.y > viewport.max.y)
            {
                float dis = go.bounds.max.y - viewport.max.y;

                dis = dis * 0.2f;

                go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - dis, go.transform.position.z);
            }

            if (go.bounds.min.x < viewport.min.x)
            {
                float dis = viewport.min.x - go.bounds.min.x;

                dis = dis * 0.2f;

                go.transform.position = new Vector3(go.transform.position.x + dis, go.transform.position.y, go.transform.position.z);
            }
            else if (go.bounds.max.x > viewport.max.x)
            {
                float dis = go.bounds.max.x - viewport.max.x;

                dis = dis * 0.2f;

                go.transform.position = new Vector3(go.transform.position.x - dis, go.transform.position.y, go.transform.position.z);
            }
        }
    }

    private float Get(float _v)
    {
        float result = 1 - _v / vv;

        if (result < 0)
        {
            result = 0;
        }

        return result;
    }
}
