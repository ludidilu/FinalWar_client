using UnityEngine;
using UnityEngine.UI;
using superRaycast;
using System;
using superGraphicRaycast;

public class AlertPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    private bool hasDown = false;

    private Action callBack;

    public void Alert(string _str, Action _callBack)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);

            SuperGraphicRaycast.SetIsOpen(false, "a");

            SuperRaycast.SetIsOpen(false, "a");
        }

        callBack = _callBack;

        alertText.text = _str;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hasDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (hasDown)
            {
                hasDown = false;

                Close();
            }
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);

        SuperGraphicRaycast.SetIsOpen(true, "a");

        SuperRaycast.SetIsOpen(true, "a");

        if (callBack != null)
        {
            callBack();
        }
    }
}
