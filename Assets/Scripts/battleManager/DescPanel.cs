using UnityEngine;
using UnityEngine.UI;
using System;

public class DescPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    private Action callBack;

    public void Show(string _str, Action _callBack)
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
