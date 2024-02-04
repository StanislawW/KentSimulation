using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointColorChanger : MonoBehaviour
{
	public Material frontMaterial;
	public Material backMaterial;
	public Renderer renderer;

	private void Update()
	{
		if(Input.GetKey(KeyCode.R))
		{
			Destroy(this.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		renderer.material = backMaterial;
	}

	private void OnTriggerExit(Collider other)
	{
		renderer.material = frontMaterial;
	}
}
