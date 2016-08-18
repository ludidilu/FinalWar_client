using UnityEngine;
using System.Collections;

public class MeshOutlineCameraScript : MonoBehaviour {
	
	[SerializeField]
	private Material mat;
	
	private Camera rc;
	
	// Use this for initialization
	void Start () {
		
		rc = GetComponent<Camera>();
		
		rc.clearFlags = CameraClearFlags.SolidColor;
		
		rc.backgroundColor = new Color(0,0,0,0);
		
		rc.depthTextureMode = DepthTextureMode.Depth;
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dest){
		
		Graphics.Blit(src,null,mat);
	}
}
