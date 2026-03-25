using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// @kurtdekker
// watches a slider, drives alpha to a RawImage and a Renderer.
//

public class DriveAlpha : MonoBehaviour
{
	[Header( "Input")]
	public Slider Sldr;

	[Header( "Output")]
	public Renderer Rndrr;
	public RawImage RawImg;

	Material cloneMaterial;

	void Start ()
	{
		// so we don't muck with what's on disk
		cloneMaterial = new Material( Rndrr.material);	
		Rndrr.material = cloneMaterial;

		// setup the slider
		Sldr.value = 1.0f;

		// drive the targets
		OnValueChanged( Sldr.value);
	}

	public void OnValueChanged( float v)
	{
		Color c = new Color( 1,1,1,v);

		// for the rendered portion
		cloneMaterial.color = c;

		// for the UI portion
		RawImg.color = c;
	}
}
