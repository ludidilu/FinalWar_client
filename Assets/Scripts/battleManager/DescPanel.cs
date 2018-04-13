using UnityEngine;
using UnityEngine.UI;

public class DescPanel : MonoBehaviour
{
    [SerializeField]
    private Text alertText;

    [SerializeField]
    private CanvasGroup cg;

    public void Show(string _str)
    {
        if (!cg.blocksRaycasts)
        {
            cg.alpha = 1;

            cg.blocksRaycasts = true;
        }

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
        }
    }
}
