using UnityEngine;

public class Fps : MonoBehaviour
{
    private GUIStyle gs;

    private int num;

    private float dt;

    private string text;

    void Awake()
    {
        gs = new GUIStyle();

        gs.fontSize = 40;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), text, gs);
    }

    void Update()
    {
        num++;

        dt += Time.deltaTime;

        if (dt > 1)
        {
            text = num.ToString();

            num = 0;

            dt -= 1;
        }
    }
}
