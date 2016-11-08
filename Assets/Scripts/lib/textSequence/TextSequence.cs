using UnityEngine;
using System.Collections;
using xy3d.tstd.lib.superTween;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using xy3d.tstd.lib.effect;

public class TextSequence
{
    private Dictionary<Text, SequenceHandler> m_sequenceDic = new Dictionary<Text, SequenceHandler>();

    private static TextSequence m_Instance;
    public static TextSequence Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new TextSequence();
            return m_Instance;
        }
    }

    class SequenceHandler
    {
        private Text text;
        private TextSequenceEffect effect;
        private string str;
        private int tweenIndex;
        private int currentIndex;
        private Action callBack;

        public SequenceHandler(Text text, TextSequenceEffect _effect, string str, Action _callBack, float time)
        {
            this.text = text;
            effect = _effect;
            this.str = str;
            callBack = _callBack;

            text.text = str;

            Start(time);
        }

        private void Start(float time)
        {
            int length = str.Length;

            effect.SetShowNum(currentIndex);

            tweenIndex = SuperTween.Instance.To(0, length, time, Step, Over);
        }

        private void Step(float value)
        {
            int index = Mathf.RoundToInt(value);

            if (index == currentIndex)
            {

                return;
            }

            currentIndex = index;

            effect.SetShowNum(currentIndex);

            text.text = "";

            text.text = str;
        }

        private void Over()
        {

            effect.SetShowNum(-1);

            text.text = "";

            text.text = str;

            callBack();
        }

        public void Stop()
        {
            SuperTween.Instance.Remove(tweenIndex);

            effect.SetShowNum(-1);

            text.text = "";

            text.text = str;

            callBack();
        }
    }

    private const float m_SingleDefualtTime = 0.12f;//单个字符出现的时间

    public void AddSequence(Text text, TextSequenceEffect _effect, string str, Action _callBack)
    {
        float time = str.Length * m_SingleDefualtTime;

        AddSequence(text, _effect, str, _callBack, time);
    }

    public void AddSequence(Text text, TextSequenceEffect _effect, string str, Action _callBack, float time)
    {
        text.text = string.Empty;

        Action dele = delegate ()
        {

            m_sequenceDic.Remove(text);

            if (_callBack != null)
            {

                _callBack();
            }
        };

        SequenceHandler handler = new SequenceHandler(text, _effect, str, dele, time);

        m_sequenceDic.Add(text, handler);
    }

    public void RemoveSequence(Text text)
    {
        if (m_sequenceDic.ContainsKey(text))
        {

            SequenceHandler handler = m_sequenceDic[text];

            handler.Stop();
        }
    }
}
