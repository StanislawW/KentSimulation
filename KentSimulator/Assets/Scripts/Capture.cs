using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief klasa Capture
 *
 * Klasa obsługująąca generację projekcji.
 * Modyfikuje wygląd ekranu na krótki czas i robi zrzut ekranu.
 *
 * \version wersja 1.0
 */
public class Capture : MonoBehaviour
{
	private int i = 0; /**< Numer zrzutu ekranu */
	public GameObject canvas; /**< canvas */
	public GameObject props; /**< GameObject props */
	public GameObject lines; /**< GameObject tła projekcji */

	public float screenshotTime; /**< czas wygaszenia interfejsu do zrobienia zrzutu ekranu */
	private float counter = -1; /**< zmienna do mierzenia czasu */
	private Color camColor; /**< Kolor tła kamery */

	/**
	 * \brief Wykonuje się na Starcie programu.
	 *
	 * Wykonuje się na Starcie programu.
	 */
	private void Start()
	{
		camColor = Camera.main.backgroundColor;
	}

	/**
	 * \brief Wykonuje się co klatkę.
	 *
	 * Co klatkę: aktualzuje zegar i sprawdza zakończenie procesu zrzutu ekranu.
	 */
	private void Update()
	{
		if(counter > 0)
		{
			counter -= Time.deltaTime;
			if (counter < 0)
				EndCapture();
		}
	}

	/**
	 * \brief Rozpoczęcie procesu zrzutu ekranu.
	 *
	 * Rozpoczyna proces zrzutu ekranu.
	 */
	public void StartCapture()
	{
		canvas.SetActive(false);
		props.SetActive(false);
		lines.SetActive(true);
		Camera.main.backgroundColor = Color.white;
		CaptureScreen();
		counter = screenshotTime;
	}

	/**
	 * \brief Zakończenie procesu zrzutu ekranu.
	 *
	 * Zakończenie procesu zrzutu ekranu.
	 */
	private void EndCapture()
	{
		lines.SetActive(false);
		canvas.SetActive(true);
		props.SetActive(true);
		Camera.main.backgroundColor = camColor;
	}

	/**
	 * \brief Zapisanie zrzutu ekranu.
	 *
	 * Zapisuje zrzut ekranu w plikach projektu
	 * Zwiększa numer zrzutu ekranu.
	 */
	private void CaptureScreen()
	{
		ScreenCapture.CaptureScreenshot("../kent_" + i + ".png");
		i++;
	}
}
