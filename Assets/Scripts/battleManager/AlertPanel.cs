using UnityEngine;
using UnityEngine.UI;
using superRaycast;
using System;
using superGraphicRaycast;

public class AlertPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    [SerializeField]
    private CanvasGroup cg;

    private Action callBack;

    private bool isDown = false;

    public void Alert(string _str, Action _callBack)
    {
        if (!cg.blocksRaycasts)
        {
            SuperRaycast.SetIsOpen(false, "AlertPanel");

            SuperGraphicRaycast.SetIsOpen(false, "AlertPanel");

            cg.alpha = 1;

            cg.blocksRaycasts = true;
        }

        callBack = _callBack;

        alertText.text = _str;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDown)
            {
                isDown = false;

                Close();
            }
        }
    }

    public void Close()
    {
        if (cg.blocksRaycasts)
        {
            SuperRaycast.SetIsOpen(true, "AlertPanel");

            SuperGraphicRaycast.SetIsOpen(true, "AlertPanel");

            cg.alpha = 0;

            cg.blocksRaycasts = false;

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
