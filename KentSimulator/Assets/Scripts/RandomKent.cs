using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class RandomKent : MonoBehaviour
{
	public Vector3 gamma1 = new Vector3(0, 0, 1);
	public Vector3 gamma2 = new Vector3(1, 0, 0);
	public Vector3 gamma3 = new Vector3(0, 1, 0);

	public float kappa; //Concentration Parameter
	public float beta; //Ovalness
	public int numberOfSamples;
	public float precision = 0.001f;

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
		StartCoroutine(GenerateCorutine());
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.R))
		{
			StopAllCoroutines();
			Debug.Log("STOP");
		}
    }

	IEnumerator GenerateCorutine()
	{
		float maxR1 = CheckR1(FindMaxU1(0, 1));
		float maxR2 = CheckR2(FindMaxU2(0, 1));
		for (int i = 0; i < numberOfSamples; i++)
		{
			Debug.Log(maxR1);
			Debug.Log(maxR2);
			Debug.Log(i + " / " + numberOfSamples);
			float u1 = Random.Range(0f, maxR1);
			float u2 = Random.Range(0f, maxR2);


			float r1;
			float r2;
			float r;

			do
			{
				yield return null;
				if (lambda1 != 0)
				{
					r1 = GenerateExponential(lambda1);
				}
				else r1 = Random.Range(0f, 1f);

				if (lambda2 != 0)
				{
					r2 = GenerateExponential(lambda2);
				}
				else r2 = Random.Range(0f, 1f);

				r = r1 * r1 + r2 * r2;

			} while (r >= 1 || !AcceptR1(u1, r1) || !AcceptR2(u2, r2));

			r1 = Random.Range(0f, 1f) < 0.5 ? r1 : -r1;
			r2 = Random.Range(0f, 1f) < 0.5 ? r2 : -r2;
			
			AddPoint(CalculateVector(r1, r2, Mathf.Sqrt(r)), i);
		}
		pointParent.transform.rotation = Quaternion.LookRotation(gamma3, gamma1);
		generated = true;
	}

	private float FindMaxU1(float min, float max)
	{
		float between = max - min;
		float halfPoint = min + between / 2f;
		if (between <= precision) return halfPoint;
		float resultMin = CheckR1(min);
		float resultMax = CheckR1(max);
		if (resultMin > resultMax) return FindMaxU1(min, halfPoint);
		else return FindMaxU1(halfPoint, max);
	}

	private float FindMaxU2(float min, float max)
	{
		float between = max - min;
		float halfPoint = min + between / 2f;
		if (between <= precision) return halfPoint;
		float resultMin = CheckR2(min);
		float resultMax = CheckR2(max);
		if (resultMin > resultMax) return FindMaxU2(min, halfPoint);
		else return FindMaxU2(halfPoint, max);
	}

	private void DestroyPoints()
	{
		foreach (GameObject p in points)
			Destroy(p);
		generated = false;
		points = null;
		pointParent.transform.rotation = Quaternion.identity;
	}
	/*
	private Vector3 GenerateKentSample()
	{
		float u1 = Random.Range(0f, 1f);
		float u2 = Random.Range(0f, 1f);

		float r1;
		float r2;
		
		float r;

		do
		{

			if (lambda1 != 0)
			{
				r1 = GenerateExponential(lambda1);
				//r1 = Random.Range(0f, 1f) < 0.5 ? r1 : -r1;
			}
			else r1 = Random.Range(0f, 1f);

			if (lambda2 != 0)
			{
				//r2 = GenerateExponential(lambda2);
				r2 = GenerateExponential2(lambda2, r1);
				//r2 = Random.Range(0f, 1f) < 0.5 ? r2 : -r2;
			}
			else r2 = Random.Range(0f, 1f);

			r = r1 * r1 + r2 * r2;
			

		} while (r >= 1 || !AcceptR1(u1, r1) || !AcceptR2(u2, r2));

		r1 = Random.Range(0f, 1f) < 0.5 ? r1 : -r1;
		r2 = Random.Range(0f, 1f) < 0.5 ? r2 : -r2;
		return CalculateVector(r1, r2, Mathf.Sqrt(r));
	}
	*/
	private bool AcceptR1(float u1, float r1)
	{
		return u1 <= CheckR1(r1);
	}

	private float CheckR1(float r1)
	{
		return Mathf.Exp(-(a * Mathf.Pow(r1, 2) + lambda1 * Mathf.Pow(r1, 4)) / 2 + lambda1 * r1 - 1);
	}

	private bool AcceptR2(float u2, float r2)
	{
		return u2 <= CheckR2(r2);
	}

	private float CheckR2(float r2)
	{
		return Mathf.Exp(-(b * Mathf.Pow(r2, 2) - gamma * Mathf.Pow(r2, 4)) / 2 + lambda2 * r2 - c2);
	}

	private float GenerateExponential(float rate)
	{
		float x = Random.Range(0f, 1f);
		return -1 / rate * Mathf.Log(1 - x);
	}

	/*
	private float GenerateExponential2(float rate, float r1)
	{
		float max;
		float min;
		if(rate >= 1)
		{
			min = Mathf.Max(1 - Mathf.Exp((rate - rate * r1 * r1 - 1 / rate) / 2 * rate - 2), 0);
			max = 1;
		}
		else
		{
			min = 0;
			max = Mathf.Min(1 - Mathf.Exp((rate - rate * r1 * r1 - 1 / rate) / 2 * rate - 2), 1);
		}

		if(min > max)
		{
			Debug.Log(min);
		}

		float x = Random.Range(min, max);
		return -1 / rate * Mathf.Log(1 - x);
	}
	*/
	private void CalculateParams()
	{
		a = 4 * kappa - 8 * beta;
		b = 4 * kappa + 8 * beta;
		gamma = 8 * beta;
		lambda1 = Mathf.Sqrt(a + 2 * Mathf.Sqrt(gamma));
		lambda2 = Mathf.Sqrt(b);
		c2 = b / (8 * kappa);
	}

	public void AddPoint(Vector3 position, int num)
	{
		GameObject newPoint = Instantiate(point, pointParent.transform);
		newPoint.name = "Point " + num;
		newPoint.transform.position = position * 10;
		points[num] = newPoint;
	}

	Vector3 CalculateVector(float x1, float x2, float r)
	{
		r = Mathf.Min(Mathf.Sqrt(x1 * x1 + x2 * x2), 1);
		//float r = Mathf.Sqrt(x1 * x1 + x2 * x2);

		float x = Mathf.Sqrt(4 * (1 - r * r) * x1 * x1);
		if (x1 < 0) x = -x;

		float y = Mathf.Sqrt(4 * (1 - r * r) * x2 * x2);
		if (x2 < 0) y = -y;

		float z = 1 - 2 * r * r;

		return  new Vector3(x, z, y);
	}
}
