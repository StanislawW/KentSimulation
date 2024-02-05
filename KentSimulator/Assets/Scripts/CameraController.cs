using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief klasa CameraController
 *
 * Klasa obsługująca ruch kamery.
 *
 * \version wersja 1.0
 */
public class CameraController : MonoBehaviour
{
	public float rotationSpeed; /**< Prędkość obrotu kamery */

	/**
	 * \brief Wykonuje się co klatkę.
	 *
	 * Co klatkę: przemieszcza kamerę kiedy wykryje przycisk W, A, S lub D.
	 */
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
