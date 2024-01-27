using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
	public LineRenderer renderer;
	public int steps;
	public float radius;

	public GameObject lineHelper;
	public int helperCount;
	public float helperLength;

	private Vector3[] helperBig;
	private Vector3[] helperSmall;

	private void Start()
	{
		DrawCircle();
		DrawHelpers();
		this.gameObject.SetActive(false);
	}

	private void DrawHelpers()
	{
		helperBig = new Vector3[helperCount];
		helperSmall = new Vector3[helperCount];

		Vector3 rad = new Vector3(radius, 0, 0);
		for (int i = 0; i < helperCount; i++)
		{
			helperBig[i] = rad;
			rad = Quaternion.AngleAxis(360f / helperCount, transform.up) * rad;
		}

		rad = new Vector3(radius - helperLength, 0, 0);
		for (int i = 0; i < helperCount; i++)
		{
			helperSmall[i] = rad;
			rad = Quaternion.AngleAxis(360f / helperCount, transform.up) * rad;
		}
		for (int i = 0; i < helperCount; i++)
		{
			GameObject newHelper = Instantiate(lineHelper, transform);
			LineRenderer newRenderer = newHelper.GetComponent<LineRenderer>();
			newRenderer.SetPosition(0, helperBig[i]);
			newRenderer.SetPosition(1, helperSmall[i]);
		}
	}

	private void DrawCircle()
	{
		Vector3 rad = new Vector3(radius, 0, 0);
		renderer.positionCount = steps;
		for (int i = 0; i < steps; i++)
		{
			renderer.SetPosition(i, rad);
			rad = Quaternion.AngleAxis(360f/steps, transform.up) * rad;
		}
	}
}
