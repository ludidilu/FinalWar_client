﻿using UnityEngine;
using UnityEngine.UI;
using superRaycast;
using System;
using superGraphicRaycast;

public class AlertPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    private Action callBack;

    private bool isDown = false;

    public void Alert(string _str, Action _callBack)
    {
        if (!gameObject.activeSelf)
        {
            SuperRaycast.SetIsOpen(false, "AlertPanel");

            SuperGraphicRaycast.SetIsOpen(false, "AlertPanel");

            gameObject.SetActive(true);
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
        if (gameObject.activeSelf)
        {
            SuperRaycast.SetIsOpen(true, "AlertPanel");

            SuperGraphicRaycast.SetIsOpen(true, "AlertPanel");

            gameObject.SetActive(false);

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
