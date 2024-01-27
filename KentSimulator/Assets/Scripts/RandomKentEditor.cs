using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomKent))]
public class RandomKentEditor : Editor
{
	public override void OnInspectorGUI()
	{
		RandomKent randomKent = (RandomKent)target;
		//randomKent.kappa = EditorGUILayout.FloatField("Concentration (kappa)", randomKent.kappa);
		//randomKent.beta = EditorGUILayout.FloatField("Ovalness (beta)", randomKent.beta);
		//randomKent.numberOfSamples = EditorGUILayout.IntField("Number of Samples", randomKent.numberOfSamples);

		DrawDefaultInspector();


		if (GUILayout.Button("Simulate Kent Distribution"))
		{
			if(Application.isPlaying)randomKent.Generate();
		}
	}
}
