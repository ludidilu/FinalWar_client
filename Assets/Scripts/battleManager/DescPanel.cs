using UnityEngine;
using UnityEngine.UI;
using System;

public class DescPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    [SerializeField]
    private CanvasGroup cg;

    private Action callBack;

    public void Show(string _str, Action _callBack)
    {
        if (!cg.blocksRaycasts)
        {
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
            Close();
        }
    }

    public void Close()
    {
        if (cg.blocksRaycasts)
        {
            cg.alpha = 0;

            cg.blocksRaycasts = false;

            if (callBack != null)
            {
                callBack();
            }
        }
    }
}
