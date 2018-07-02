using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPoint : MonoBehaviour {

	public Texture2D texture;
    private Vector2 pointPos;

	private void Start()
	{
        pointPos = new Vector2(Screen.width / 10.0f, Screen.height / 10.0f);
	}

	// Use this for initialization
	void OnGUI()
	{
		Rect rect = new Rect(
            pointPos.x - (texture.width/2),
            (Screen.height - pointPos.y) - (texture.height/2),
			texture.width, 
			texture.height
		);
		GUI.DrawTexture(rect, texture);
	}
}
