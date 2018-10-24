using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underwater : MonoBehaviour {

	public float waterLevel;
	private bool isUnderwater;
	public Color normalColor;
	public Color underwaterColor;
	

	void Update () {

		isUnderwater = true;

		if (isUnderwater)
			SetUnderwater ();
		if (!isUnderwater)
			SetNormal ();

	}

	void SetNormal () {

		RenderSettings.fogColor = normalColor;
		RenderSettings.fogDensity = 0.002f;
	}

	void SetUnderwater () {

		RenderSettings.fogColor = underwaterColor;
		RenderSettings.fogDensity = 0.012f;

	}

}
