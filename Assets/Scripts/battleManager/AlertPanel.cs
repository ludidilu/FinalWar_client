using UnityEngine;
using UnityEngine.UI;
using superRaycast;
using System;
using superGraphicRaycast;

public class AlertPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    private Action callBack;

    public void Alert(string _str, Action _callBack)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        callBack = _callBack;

        alertText.text = _str;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    public void Close()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
