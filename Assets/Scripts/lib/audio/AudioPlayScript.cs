using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayScript : MonoBehaviour {

	private List<AudioSource> effectSources = new List<AudioSource>();

	private float pitch = 1;

	public void PlayEffect(AudioClip _clip){

		AudioSource source = gameObject.AddComponent<AudioSource>();

		source.clip = _clip;

		source.pitch = pitch;

        source.volume = xy3d.tstd.lib.audio.AudioPlay.Instance.PlayEffectVol;
		source.Play();
		effectSources.Add(source);
	}
    public float SetPlayEffectVol
    {
        get
        {
            if (effectSources.Count <= 0) return 0;
            return effectSources[0].volume;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 1);

			for(int i = 0; i < effectSources.Count ; i++){

				effectSources[i].volume = value;
            }
        }
    }
	public void SetPlayEffectSpeed(float _speed){

		pitch = _speed;

		for(int i = 0 ; i < effectSources.Count ; i++){

			effectSources[i].pitch = pitch;
		}
	}

	// Update is called once per frame
	void Update () {
	
		for(int i = effectSources.Count - 1 ; i > -1 ; i--){

			if(!effectSources[i].isPlaying){

				GameObject.Destroy(effectSources[i]);

				effectSources.RemoveAt(i);
			}
		}
	}
}
