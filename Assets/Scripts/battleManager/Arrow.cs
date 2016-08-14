using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

	[SerializeField]
	private Image img;

	[SerializeField]
	private bool replaceMat;

	// Use this for initialization
	void Awake () {

		if (replaceMat) {
	
			img.material = GameObject.Instantiate<Material> (img.material);
		}
	}
	
	public void SetIndex(int _index){

		img.material.SetFloat ("_Fix", _index);
	}

	public void SetColor(Color _color){

		img.color = _color;
	}
}
