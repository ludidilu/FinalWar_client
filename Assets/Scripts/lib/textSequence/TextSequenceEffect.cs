#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace xy3d.tstd.lib.effect{
	
	[AddComponentMenu("UI/Effects/TextSequenceEffect")]
	public class TextSequenceEffect : BaseMeshEffect
	{
		private int showNum = -1;

		public void SetShowNum(int _showNum){

			showNum = _showNum;
		}

		#if UNITY_5_2_2 || UNITY_5_2_3 || UNITY_5_3 || UNITY_5_4
		public override void ModifyMesh (VertexHelper vh){
			
			if (!IsActive () || showNum == -1) {
				
				return;
			}

			int index = 0;
			
			for(int i = 0 ; i < vh.currentIndexCount / 6 ; i++){
				
				UIVertex uiVertex0 = new UIVertex();
				
				vh.PopulateUIVertex(ref uiVertex0,i * 4);
				
				bool isSame = true;
				
				for(int m = 1 ; m < 4 ; m++){
					
					UIVertex uiVertex1 = new UIVertex();
					
					vh.PopulateUIVertex(ref uiVertex1,i * 4 + m);
					
					if(uiVertex1.position != uiVertex0.position){
						
						isSame = false;
						
						break;
					}
				}
				
				if(!isSame){
					
					if(index < showNum){
						
						
					}else{
						
						for(int m = 0 ; m < 4 ; m++){
							
							UIVertex uiVertex1 = new UIVertex();
							
							vh.PopulateUIVertex(ref uiVertex1,i * 4 + m);
							
							uiVertex1.color = new Color32(0,0,0,0);
							
							vh.SetUIVertex(uiVertex1,i * 4 + m);
						}
					}
					
					index++;
				}
			}
		}
		
		#endif

		public override void ModifyMesh (Mesh _mesh)
		{
			if (!IsActive () || showNum == -1) {
				
				return;
			}
			
			using (VertexHelper vh = new VertexHelper(_mesh)) {
					
				int index = 0;
				
				for(int i = 0 ; i < vh.currentIndexCount / 6 ; i++){
					
					UIVertex uiVertex0 = new UIVertex();
					
					vh.PopulateUIVertex(ref uiVertex0,i * 4);
					
					bool isSame = true;
					
					for(int m = 1 ; m < 4 ; m++){
						
						UIVertex uiVertex1 = new UIVertex();
						
						vh.PopulateUIVertex(ref uiVertex1,i * 4 + m);
						
						if(uiVertex1.position != uiVertex0.position){
							
							isSame = false;
							
							break;
						}
					}
					
					if(!isSame){
						
						if(index < showNum){
							
							
						}else{
							
							for(int m = 0 ; m < 4 ; m++){
								
								UIVertex uiVertex1 = new UIVertex();
								
								vh.PopulateUIVertex(ref uiVertex1,i * 4 + m);
								
								uiVertex1.color = new Color32(0,0,0,0);
								
								vh.SetUIVertex(uiVertex1,i * 4 + m);
							}
						}
						
						index++;
					}
				}

				vh.FillMesh (_mesh);
			}
		}
		
	}
}

#else

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace xy3d.tstd.lib.effect{
	
	[AddComponentMenu("UI/Effects/Gradient")]
	public class TextSequenceEffect : BaseVertexEffect {

		private int showNum = -1;
		
		public void SetShowNum(int _showNum){
			
			showNum = _showNum;
		}
		
		public override void ModifyVertices(List<UIVertex> vertexList) {

			if (!IsActive() || showNum == -1) {

				return;
			}

			int index = 0;

			for(int i = 0 ; i < vertexList.Count < 4 ; i++){

				UIVertex uiVertex0 = vertexList[i * 4];
				
				bool isSame = true;
				
				for(int m = 1 ; m < 4 ; m++){
					
					UIVertex uiVertex1 = vertexList[i * 4 + m];

					if(uiVertex1.position != uiVertex0.position){
						
						isSame = false;
						
						break;
					}
				}
				
				if(!isSame){
					
					if(index < showNum){
						
						
					}else{
						
						for(int m = 0 ; m < 4 ; m++){
							
							UIVertex uiVertex1 = vertexList[i * 4 + m];
							
							uiVertex1.color = new Color32(0,0,0,0);
							
							vertexList[i * 4 + m] = uiVertex1;
						}
					}
					
					index++;
				}
			}
		}
	}
}
#endif
