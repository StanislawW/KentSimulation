using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKent : MonoBehaviour
{
	public Vector3 gamma1 = new Vector3(0, 0, 1);
	public Vector3 gamma2 = new Vector3(1, 0, 0);
	public Vector3 gamma3 = new Vector3(0, 1, 0);

	public float kappa; //Concentration Parameter
	public float beta; //Ovalness
	public int numberOfSamples;

	public GameObject point;
	public GameObject pointParent;

	public GameObject[] points;

	private float a;
	private float b;
	private float gamma;
	private float lambda1;
	private float lambda2;
	private float c2;

	private bool generated = false;


	public void Generate()
	{
		if (generated) DestroyPoints();

		CalculateParams();
		points = new GameObject[numberOfSamples];
		for (int i = 0; i < numberOfSamples; i++)
		{
			AddPoint(GenerateKentSample(), i);
		}
		pointParent.transform.rotation = Quaternion.LookRotation(gamma3, gamma1);
		generated = true;
	}

	private void DestroyPoints()
	{
		foreach (GameObject p in points)
			Destroy(p);
		generated = false;
		points = null;
		pointParent.transform.rotation = Quaternion.identity;
	}

	private Vector3 GenerateKentSample()
	{
		float u1 = Random.Range(0f, 1f);
		float u2 = Random.Range(0f, 1f);

		float r1;
		float r2;

		do
		{
			if (lambda1 != 0)
			{
				r1 = GenerateExponential(lambda1);
				r1 = Random.Range(0f, 1f) < 0.5 ? r1 : -r1;
			}
			else r1 = Random.Range(-1f, 1f);

			if (lambda2 != 0)
			{
				r2 = GenerateExponential(lambda2);
				r2 = Random.Range(0f, 1f) < 0.5 ? r2 : -r2;
			}
			else r2 = Random.Range(-1f, 1f);

		} while (AcceptR1(u1, r1) && AcceptR2(u2, r2) && r1 * r1 + r2 * r2 < 1);

		return CalcPos(r1, r2);

		/*
		float cosTheta = 1 - 2 * (r1 * r1 + r2 * r2);
		float sinPhi = r2 / Mathf.Sqrt(r1 * r1 + r2 * r2);
		float cosPhi = r1 / Mathf.Sqrt(r1 * r1 + r2 * r2);

		return new Vector3(Mathf.Sqrt(1 - cosTheta * cosTheta) * cosPhi, cosTheta, Mathf.Sqrt(1 - cosTheta * cosTheta) * sinPhi);
		*/
	}

	private bool AcceptR1(float u1, float r1)
	{
		return u1 <= Mathf.Exp(-(a * Mathf.Pow(r1, 2) + lambda1 * Mathf.Pow(r1, 4)) / 2 + lambda1 * r1 - 1);
	}

	private bool AcceptR2(float u2, float r2)
	{
		return u2 <= Mathf.Exp(-(b * Mathf.Pow(r2, 2) + gamma * Mathf.Pow(r2, 4)) / 2 + lambda2 * r2 - c2);
	}

	private float GenerateExponential(float beta)
	{
		float x = Random.Range(0f, 1f);
		return -1 / beta * Mathf.Log(1 - x);
	}

	private void CalculateParams()
	{
		a = 4 * kappa - 8 * beta;
		b = 4 * kappa + 8 * beta;
		gamma = 8 * beta;
		lambda1 = Mathf.Sqrt(a + 2 * Mathf.Sqrt(gamma));
		lambda2 = Mathf.Sqrt(b);
		c2 = b / 8 * kappa;
	}

	public void AddPoint(Vector3 position, int num)
	{
		GameObject newPoint = Instantiate(point, pointParent.transform);
		newPoint.name = "Point " + num;
		newPoint.transform.position = position * 10;
		points[num] = newPoint;
	}

	Vector3 CalcPos(float x1, float x2)
	{
		float r = Mathf.Min(Mathf.Sqrt(x1 * x1 + x2 * x2), 1);

		float x = Mathf.Sqrt(4 * (1 - r * r) * x1 * x1);
		if (x1 < 0) x = -x;

		float y = Mathf.Sqrt(4 * (1 - r * r) * x2 * x2);
		if (x2 < 0) y = -y;

		float z = 1 - 2 * r * r;

		return  new Vector3(x, z, y);
	}
}
