// Shuriken Particle System Scale Tool
// Version: 1.0.0
// Copyright (c) 2015 Benn Lockyer (@twistedshield)
// Support: www.twistedshield.com/tools

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class ParticleScaleTool : EditorWindow
{
	//Put the window in the tools menu
	[MenuItem("Tools/Particle Scale Tool")]
	
	//Gets called to open the window
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(ParticleScaleTool));
	}
	
	//GameObject for our currently selected editor object
	public GameObject sel;
	//Does the object have a particle system?
	private bool hasParticleSystem;
	//Live scale
	private float scale;
	//This holds the current object, makes sure everything is reset if we change
	private GameObject ready;
	
	//Variables for the particle system
	private List<float> startSpeed = new List<float>();
	private List<float> startLife = new List<float>();
	private List<float> startSize = new List<float>();
	private List<float> startRate = new List<float>();
	private List<float> startGrav = new List<float>();
	private List<float> startMax = new List<float>();
	
	//Object's scale
	private Vector3 startScale;
	
	//Bools to check if things are getting scaled
	private bool scaleSpeed = true;
	private bool scaleLife = true;
	private bool scaleSize = true;
	private bool scaleRate = true;
	private bool scaleGrav = true;
	private bool scaleMax = true;
	private bool scaleEmission = true;
	
	//String to show text to the user
	private string displayText1;
	private string displayText2;
	
	void OnGUI()
	{
		//Show the game object to the user
		GUILayout.BeginHorizontal();
		sel = (GameObject)EditorGUILayout.ObjectField(Selection.activeGameObject,typeof(GameObject),true);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		//String to show text
		
		//If we have no object selected
		if(sel == null)
		{
			hasParticleSystem = false;
			//Check if we had an old object, and revert changes if we did
			EmptyPrevious();
			//tell the user they have nothing selected
			displayText1 = "NO GAME OBJECT SELECTED";
			displayText2 = "";
		}
		else //We have an object selected
		{
			//See if the object has a particle system
			if(sel.GetComponent<ParticleSystem>())
			{
				//Particle system found
				hasParticleSystem = true;
				
				//If this is a new object, empty the last one
				if(sel != ready)
				{
					//Check if we had an old object, and revert changes if we did
					EmptyPrevious();
					//Show the use the object is valid
					displayText1 = "VALID GAME OBJECT";
					displayText2 = "PRESS 'ALLOW SCALING' TO BEGIN";
				}
				else
				{
					displayText1 = "SCALING ENABLED";
					displayText2 = "";
				}
				
			}
			else //Object has no particle system
			{
				hasParticleSystem = false;
				//Inform the user
				displayText1 = "SELECTED GAME OBJECT DOES NOT HAVE AN";
				displayText2 = "ATTACHED SHURIKEN PARTICLE SYSTEM";
				//Check if we had an old object, and revert changes if we did
				EmptyPrevious();
			}
		}
		
		//Show the text to the user
		GUILayout.Label(displayText1,GUILayout.Width(300));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(displayText2,GUILayout.Width(300));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		//No previous object, and current has particles
		if(ready == null && hasParticleSystem)
		{
			//Show a button to allow scaling
			if(GUILayout.Button("ALLOW SCALING"))
			{
				//Get all references and save variables
				Apply();
			}
		}
		//We have particle system, and have grabbed all references (applied)
		else if(ready != null && hasParticleSystem)
		{
			//Show a bunch of tick boxes to give user some control of what will scale
			scaleSpeed = EditorGUILayout.Toggle("Scale Speed?",scaleSpeed);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleLife = EditorGUILayout.Toggle("Scale Lifetime?",scaleLife);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleSize = EditorGUILayout.Toggle("Scale Size?",scaleSize);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleRate = EditorGUILayout.Toggle("Scale Emission Rate?",scaleRate);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleGrav = EditorGUILayout.Toggle("Scale Gravity?",scaleGrav);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleMax = EditorGUILayout.Toggle("Scale Max Particles?",scaleMax);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			scaleEmission = EditorGUILayout.Toggle("Scale Emission Shape?",scaleEmission);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			//Get reference to the particle system
			ParticleSystem p = sel.GetComponent<ParticleSystem>();
			//Draw a slider bar for the scale (0 - 10)
			scale = EditorGUILayout.Slider("Scale",scale,0f,10f);
			//Update particle system
			UpdateScale(p,0);
			//Scale stays outside to prevent child transforms from extra scale
			if(scaleEmission)
				sel.transform.localScale = startScale * scale;
			//See how many children we have
			int chCount = sel.transform.childCount;
			
			//If we have children
			if(chCount > 0)
			{
				int count = 1;
				//Count through them
				for(int x = 0; x < chCount; ++x)
				{
					//If they have particle systems
					if(sel.transform.GetChild(x).GetComponent<ParticleSystem>())
					{
						//Scale those systems, too!
						UpdateScale(sel.transform.GetChild(x).GetComponent<ParticleSystem>(), count);
						++count;
					}
				}
			}
			
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			//If we have adjusted our scale give the user some options
			if(scale != 1)
			{
				//APPLY button - save changes and apply them to the particle system
				if(GUILayout.Button("APPLY",GUILayout.Width(100)))
				{
					Apply();
				}
				//REVERT button - discard changes and rese particle system
				if(GUILayout.Button("REVERT",GUILayout.Width(100)))
				{
					Revert();
				}
			}
		}
	}
	
	void Revert()
	{	
		ParticleSystem p = ready.GetComponent<ParticleSystem>();
		//Default scale to 1
		scale = 1.0f;
		//Reset values in the particle system
		p.startSpeed = startSpeed[0];
		p.startLifetime = startLife[0];
		p.startSize = startSize[0];
		p.emissionRate = startRate[0];
		p.gravityModifier = startGrav[0];
		p.maxParticles = (int)startMax[0];
		
		ready.transform.localScale = startScale;
		int count = 1;
		//Get default values for children	
		if(ready.transform.childCount > 0)
		{
			for(int x = 0; x < ready.transform.childCount; ++x)
			{
				if(ready.transform.GetChild(x).GetComponent<ParticleSystem>())
				{
					
					ParticleSystem cp = ready.transform.GetChild(x).GetComponent<ParticleSystem>();
					cp.startSpeed = startSpeed[count];
					cp.startLifetime = startLife[count];
					cp.startSize = startSize[count];
					cp.emissionRate = startRate[count];
					cp.gravityModifier = startGrav[count];
					cp.maxParticles = (int)startMax[count];
					++count;
				}
			}
		}		
	}
	
	void Apply()
	{
		//Reset arrays
		startSpeed = new List<float>();
		startLife = new List<float>();
		startSize = new List<float>();
		startRate = new List<float>();
		startGrav = new List<float>();
		startMax = new List<float>();
		
		//Get reference to the objects particle system
		ParticleSystem p = sel.GetComponent<ParticleSystem>();
		
		//Set ready to our current object
		ready = sel;
		
		//Get values from the particle system
		startSpeed.Add(p.startSpeed);
		startLife.Add(p.startLifetime);
		startSize.Add(p.startSize);
		startRate.Add(p.emissionRate);
		startGrav.Add(p.gravityModifier);
		startMax.Add(p.maxParticles);
		sel.transform.localScale = new Vector3(1,1,1);
		startScale = sel.transform.localScale;
		//Get default values for children	
		if(sel.transform.childCount > 0)
		{
			for(int x = 0; x < sel.transform.childCount; ++x)
			{
				if(sel.transform.GetChild(x).GetComponent<ParticleSystem>())
				{
					ParticleSystem cp = sel.transform.GetChild(x).GetComponent<ParticleSystem>();
					startSpeed.Add(cp.startSpeed);
					startLife.Add(cp.startLifetime);
					startSize.Add(cp.startSize);
					startRate.Add(cp.emissionRate);
					startGrav.Add(cp.gravityModifier);
					startMax.Add(cp.maxParticles);
				}
			}
		}
		//Default scale to 1
		scale = 1.0f;
		
	}
	
	//Update particle system
	void UpdateScale(ParticleSystem p, int index)
	{
		if(scaleSpeed)
			p.startSpeed = startSpeed[index] * scale;
		if(scaleLife)
			p.startLifetime = startLife[index] * scale;
		if(scaleSize)
			p.startSize = startSize[index] * scale;
		if(scaleRate)
			p.emissionRate = startRate[index] * scale;
		if(scaleGrav)
			p.gravityModifier = startGrav[index] * scale;
		if(scaleMax)
			p.maxParticles = (int)(startMax[index] * scale);
	}
	
	//Empty last object
	void EmptyPrevious()
	{
		//If we had an object, revert changes
		if(ready != null)
			Revert();
		//Drop the reference
		ready = null;
	}

	//Force update the window, even when not on focus
	public void OnInspectorUpdate()
	{
		Repaint();
	}
}
