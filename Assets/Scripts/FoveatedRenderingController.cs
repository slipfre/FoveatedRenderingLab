using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerSizes{
	// 每层的大小为 PheripheryLayer 的大小乘以这里的比例因子, PheripheryLayer 一般为 1
	public float InnerLayer;
	public float MediumLayer;
	public float PheripheryLayer;
}

[System.Serializable]
public class SamplingFactors{
	// 每层的大小为 PheripheryLayer 的大小乘以这里的比例因子, InnerLayer 一般为 1
	public float InnerLayer;
	public float MediumLayer;
	public float PheripheryLayer;
}

public class FoveatedRenderingController : MonoBehaviour {

	public Camera innerLayerCamera;
	public Camera mediumLayerCamera;
	public Camera peripheryLayerCamera;

	public LayerSizes layerSizes;
	public SamplingFactors samplingFactors;

	private RenderTexture pRT;
	private RenderTexture mRT;
	private RenderTexture iRT;

	private Material material;
	[SerializeField] private Shader textureCombineShader;

	private float mHO;
	private float mVO;
	private float iHO;
	private float iVO;

	// 设置 Camera 的渲染区域
	void CamFOVConf(Camera cam, float layerSize, Vector2 centerPoint){
		float radian = Mathf.Tan(peripheryLayerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		cam.fieldOfView = Mathf.Atan(radian * layerSize)/Mathf.Deg2Rad*2;
	}

	// 设置 Camera 需要渲染的 resolution
	RenderTexture CamRTConf(Camera cam, float layerSize, float samplingFactor, bool square){
		int nativeHeight = Screen.height;
		int nativeWidth = Screen.width;
		if (square) {
			nativeHeight = (nativeWidth > nativeHeight) ? nativeHeight : nativeWidth;
			nativeWidth = nativeHeight;
		}
		int rtHeight = (int)((nativeHeight * layerSize) / samplingFactor);
		int rtWidth = (int)((nativeWidth * layerSize) / samplingFactor);
		return RenderTexture.GetTemporary (rtWidth, rtHeight);
	}

	void CameraConfigure() {
		Vector2 center = new Vector2 (0.5f, 0.5f);
		CamFOVConf(peripheryLayerCamera, layerSizes.PheripheryLayer, center);
		CamFOVConf(mediumLayerCamera, layerSizes.MediumLayer, center);
		CamFOVConf(innerLayerCamera, layerSizes.InnerLayer, center);
		pRT = CamRTConf (peripheryLayerCamera, layerSizes.PheripheryLayer, samplingFactors.PheripheryLayer, false);
		peripheryLayerCamera.targetTexture = pRT;
		mRT = CamRTConf (mediumLayerCamera, layerSizes.MediumLayer, samplingFactors.MediumLayer, true);
		mediumLayerCamera.targetTexture = mRT;
		iRT = CamRTConf (innerLayerCamera, layerSizes.InnerLayer, samplingFactors.InnerLayer, true);
		innerLayerCamera.targetTexture = iRT;
	}

	void Awake(){
		CameraConfigure ();
		material = new Material (textureCombineShader);
	}

	void LateUpdate(){
		Vector3 center = peripheryLayerCamera.ScreenToViewportPoint (Input.mousePosition) / samplingFactors.PheripheryLayer;
		Debug.Log (center);
		iHO = (center.x - 0.5f) / (0.5f * layerSizes.InnerLayer * Screen.height / Screen.width);
		iVO = (center.y - 0.5f) / (0.5f * layerSizes.InnerLayer);
		Debug.Log (iHO);
//		iHO = (Input.mousePosition.x - Screen.width / 2.0f) / (iRT.width * samplingFactors.InnerLayer / 2.0f);
//		iVO = (Input.mousePosition.y - Screen.height / 2.0f) / (iRT.height * samplingFactors.InnerLayer / 2.0f);
		SetObliqueness (innerLayerCamera, iHO, iVO);
		mHO = (center.x - 0.5f) / (0.5f * layerSizes.MediumLayer * Screen.height / Screen.width);
		mVO = (center.y - 0.5f) / (0.5f * layerSizes.MediumLayer);
//		mHO = (Input.mousePosition.x - Screen.width / 2.0f) / (mRT.width * samplingFactors.MediumLayer / 2.0f);
//		mVO = (Input.mousePosition.y - Screen.height / 2.0f) / (mRT.height * samplingFactors.MediumLayer / 2.0f);
		SetObliqueness (mediumLayerCamera, mHO, mVO);
	}
	
	void SetObliqueness(Camera cam, float horizObl, float vertObl) {
		Matrix4x4 mat  = cam.projectionMatrix;
		mat[0, 2] = horizObl;
		mat[1, 2] = vertObl;
		cam.projectionMatrix = mat;
	}

	void OnPreRender(){
		innerLayerCamera.targetTexture = iRT;
	}

	void OnPostRender(){
		innerLayerCamera.targetTexture = null; 
	} 

	void ShaderConfigure (Texture innerTex, Texture mediumTex, Texture peripheryTex){
		LayerConfigure ("_PeripheryTex", peripheryTex, "_PeripheryArea", samplingFactors.PheripheryLayer);
		LayerConfigure ("_MediumTex", mediumTex, "_MediumArea", samplingFactors.MediumLayer);
		LayerConfigure ("_InnerTex", innerTex, "_InnerArea", samplingFactors.InnerLayer);
	}

	void LayerConfigure (string texName, Texture tex, string areaName, float samplingFactor){
		material.SetTexture (texName, tex);
		float width = tex.width * samplingFactor;
		float height = tex.height * samplingFactor;
		Vector2 offset = new Vector2 (
			Input.mousePosition.x - width / 2, Input.mousePosition.y - height / 2
		);
//		Vector2 offset = new Vector2 (
//			(Screen.width - width) / 2, (Screen.height - height) / 2
//		);
		Vector4 area = CalcArea (offset, tex, samplingFactor);
		material.SetVector (areaName, area);
	}

	// Offset 为区域左下角距离屏幕坐标系原点的距离
	Vector4 CalcArea (Vector2 offset, Texture t, float samplingFactor) {
		return new Vector4 (
			offset.x, 
			offset.y, 
			offset.x + t.width * samplingFactor - 1, 
			offset.y + t.height * samplingFactor - 1
		);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest){
		ShaderConfigure (src, mRT, pRT);
		Graphics.Blit (pRT, null as RenderTexture, material);
	}
}
