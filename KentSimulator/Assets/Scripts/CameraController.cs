using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float rotationSpeed;

	private void Update()
	{
		if(Input.GetKey(KeyCode.W))
		{
			transform.Rotate(transform.right * Time.deltaTime * rotationSpeed, Space.World);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.Rotate(-transform.right * Time.deltaTime * rotationSpeed, Space.World);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
		}
	}
}
