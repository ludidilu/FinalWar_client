using UnityEngine;
using System;

public class DamageNum : MonoBehaviour
{
    [SerializeField]
    private TextMesh text;

    [SerializeField]
    private TextMeshOutline textOutline;

    [SerializeField]
    private AnimationCurve posCurve;

    [SerializeField]
    private AnimationCurve alphaCurve;

    [SerializeField]
    private float moveTime;

    [SerializeField]
    private float height;

    private float startTime;

    private float startY;

    private Action callBack;

    public void Init(string _str, Color _color, Color _outlineColor, Action _callBack)
    {
        startTime = Time.time;

        text.text = _str;

        text.color = _color;

        textOutline.SetOutlineColor(_outlineColor);

        callBack = _callBack;

        startY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;

        float percent = (time - startTime) / moveTime;

        if (percent > 1)
        {
            Destroy(gameObject);

            if (callBack != null)
            {
                callBack();
            }
        }
        else
        {
            float value = posCurve.Evaluate(percent);

            transform.localPosition = new Vector3(transform.localPosition.x, startY + value * height, 0);

            value = alphaCurve.Evaluate(percent);

            text.color = new Color(text.color.r, text.color.g, text.color.b, value);
        }
    }
}
