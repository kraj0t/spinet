using UnityEngine;
using System.Collections;

public class LetterBox : MonoBehaviour {
	
	// Usually you'll want to use a black solid texture for this.
	public Texture2D texture;
	
	public bool showing = false;
	
	public float fadeTime = 1.0f;
	
	// Value is 1 when letterbox is fully shown.
	// Set this to 0/1 to instantaneously hide/show the letterbox, without fade-in/out
	public float currentShownAmount = 0f;
	
	// Use 0.1 for a tenth of the screen height.
	public float sizeByScreenHeight = 0.1f;
	
	
	// Update is called once per frame
	void Update () {
		if ( showing )
			currentShownAmount += fadeTime * Time.deltaTime;
		else
			currentShownAmount -= fadeTime * Time.deltaTime;
		currentShownAmount = Mathf.Clamp( currentShownAmount, 0f, 1f );
	}
	
	void StartLetterbox ()
	{
		currentShownAmount = 0;
		showing = true;
	}
	
	void OnGUI ()
	{
		GUI.depth = 10000;
		float verticalSize = Screen.height * currentShownAmount * sizeByScreenHeight;
		GUI.DrawTexture( new Rect( 0, 0, Screen.width, verticalSize ), texture );
		GUI.DrawTexture( new Rect( 0, Screen.height - verticalSize, Screen.width, verticalSize ), texture );
	}	
	
}
