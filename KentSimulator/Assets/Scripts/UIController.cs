using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * \brief klasa UIController
 *
 * Klasa obsługujóca interfejs użytkownika.
 * Dodatkowo zajmuje się wywoływaniem metod z innych klas.
 *
 * \version wersja 1.0
 */
public class UIController : MonoBehaviour
{
	[Header("General")]
	public RandomKent randomKent; /**< Klasa RandomKent */
	public Toggle showGizmos; /**< Checkbox */
	public Toggle showAxes; /**< Checkbox */
	public GameObject axesLines; /**< GameOBject linii pomocniczych osi świata */
	public Material selected; /**< Materiał wybranej linii pomocniczej */
	public Material ambient; /**< Materiał nie wybranej linii pomocniczej */
	public TMP_InputField sampleNum; /**< Pole tekstowe */
	public Button readyButton; /**< Przycisk */
	public FileHandler fileHandler; /**< Klasa FileHandler */
	public Capture capture; /**< Klasa Capture */
	public GameObject exitPopUp; /**< GameObject okientka wyłączającego prgoram */
	public Slider progressBar; /**< Slider */

	[Header("Camera Buttons")]
	public GameObject camHandle; /**< GameOBject rodzica MainCamera */
	public Button xButton; /**< Przycisk */
	public Button yButton; /**< Przycisk */
	public Button zButton; /**< Przycisk */

	[Header("Random Toggles")]
	public Toggle randomizeGamma1; /**< Checkbox */
	public Toggle randomizeGamma23; /**< Checkbox */
	public Toggle randomizeKappa; /**< Checkbox */
	public Toggle randomizeBeta; /**< Checkbox */

	[Header("Kappa Parameters")]
	public TMP_InputField kappa; /**< Pole tekstowe */

	[Header("Beta Parameters")]
	public TMP_InputField beta; /**< Pole tekstowe */

	[Header("Mean Direction Parameters")]
	public GameObject meanDirPopUp; /**< GameObject okienka ośi wielkiej/małej */
	public TMP_InputField meanDirTheta; /**< Pole tekstowe */
	public TMP_InputField meanDirPhi; /**< Pole tekstowe */
	public TMP_Text meanDirX; /**< Etykieta tekstowa */
	public TMP_Text meanDirY; /**< Etykieta tekstowa */
	public TMP_Text meanDirZ; /**< Etykieta tekstowa */
	public Button meanDirSetButton; /**< Przycisk */
	public LineRenderer meanDirLineRenderer; /**< Renderer */

	[Header("Axis Parameters")]
	public GameObject axisPopUp; /**< GameObject okientka średniego kierunku */
	public TMP_InputField axisAngle; /**< Pole tekstowe */
	public TMP_Text majorAxisX; /**< Etykieta tekstowa */
	public TMP_Text majorAxisY; /**< Etykieta tekstowa */
	public TMP_Text majorAxisZ; /**< Etykieta tekstowa */
	public TMP_Text minorAxisX; /**< Etykieta tekstowa */
	public TMP_Text minorAxisY; /**< Etykieta tekstowa */
	public TMP_Text minorAxisZ; /**< Etykieta tekstowa */
	public Button axisSetButton; /**< Przycisk */
	public LineRenderer majorAxisLineRenderer; /**< Renderer */
	public LineRenderer minorAxisLineRenderer; /**< Renderer */

	private float theta = Mathf.PI/2; /**< Zmienna pomocnicza */

	/**
	 * \brief Wykonuje się co klatkę.
	 *
	 * Co klatkę: wykrywa nacięniście przycisku Escape i otwiera okno zamknięcia gry, aktualizuje pasek postępu, wyłącza zakończony pasek postępu.
	 */
	private void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			OpenExitPopUp();
		}
		progressBar.value = (float)randomKent.i / randomKent.numberOfSamples;
		if(progressBar.value == 1) progressBar.gameObject.SetActive(false);
	}

	/**
	 * \brief Zamykanie okna zamknięcia programu.
	 *
	 * Dezaktywuje GameObject okna zakmnięcia programu.
	 */
	public void CloseExitPopUp()
	{
		exitPopUp.SetActive(false);
	}

	/**
	 * \brief Otwarcie okna zamknięcia programu.
	 *
	 * Aktywuje GameObject okna zakmnięcia programu.
	 * Zamyka inne okna.
	 */
	public void OpenExitPopUp()
	{
		exitPopUp.SetActive(true);
		CloseAxisPopup();
		CloseMeanDirPopup();
	}

	/**
	 * \brief Zamykanie aplikacji.
	 *
	 * Wyłącza aplikacj e.
	 */
	public void ExitApp()
	{
		Application.Quit();
	}

	/**
	 * \brief Wywo  lanie eksportu tekstu.
	 *
	 * Wywo  luje metod   e Export klasy FileHandler.
	 */
	public void ExportTxt()
	{
		fileHandler.Export("kent");
	}

	/**
	 * \brief Wywołanie eksportu projekcji.
	 *
	 * Wywołuje metodę StartCapture klasy Capture.
	 */
	public void ExportImg()
	{
		capture.StartCapture();
	}

	/**
	 * \brief Ustawia widok z osi X.
	 *
	 * Ustawia obrót rodzica Kamery na wektor (-90, -90, 0).
	 */
	public void XButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(-90, -90, 0);
	}
	/**
	 * \brief Ustawia widok z osi Y.
	 *
	 * Ustawia obrót rodzica Kamery na wektor (-90, -180, 0).
	 */
	public void YButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(-90, -180, 0);
	}
	/**
	 * \brief Ustawia widok z osi Z.
	 *
	 * Ustawia obrót rodzica Kamery na wektor (0, 0, 0).
	 */
	public void ZButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(0, 0, 0);
	}

	/**
	 * \brief Aktualizacja linii pomocniczych.
	 *
	 * Ustawia aktywność linii pomocniczych, osi świata, osi małej/wielkiej i średniego kierunku na wartość ustawioną przez użykownika.
	 * Wywolywane przy aktualizacji konkretnych elementów UI.
	 */
	public void ToggleGizmos()
	{
		axesLines.SetActive(showAxes.isOn);
		meanDirLineRenderer.gameObject.SetActive(showGizmos.isOn);
		majorAxisLineRenderer.gameObject.SetActive(showGizmos.isOn);
		minorAxisLineRenderer.gameObject.SetActive(showGizmos.isOn);
	}

	/**
	 * \brief Aktualizacja gotowości do generacji.
	 *
	 * Aktualizuje stan gotowości do generacji próbek.
	 * Pola muszą być wypełnione będą ustawione na losowe.
	 * Aktualizuje stan przycisku do generacji.
	 */
	public void CheckReady()
	{
		bool ready = true;

		if (string.IsNullOrEmpty(meanDirX.text) && !randomizeGamma1.isOn) ready = false;
		if (string.IsNullOrEmpty(majorAxisX.text) && !randomizeGamma23.isOn) ready = false;
		if (string.IsNullOrEmpty(minorAxisX.text) && !randomizeGamma23.isOn) ready = false;
		if (string.IsNullOrEmpty(kappa.text) && !randomizeKappa.isOn) ready = false;
		if (string.IsNullOrEmpty(beta.text) && !randomizeBeta.isOn) ready = false;
		
		readyButton.interactable = ready;
	}

	/**
	 * \brief Wywołanie generacji.
	 *
	 * Losuje parametry które są ustawione na losowe.
	 * Wywołuje aktualizację liczby próbek.
	 * Aktywuje pasek postępu.
	 * Wywołuje metodę Generate klasy RandomKent celem rozpoczęcia genracji.
	 */
	public void Run()
	{
		if(randomizeGamma1.isOn)
		{
			meanDirTheta.text = Random.Range(0f, 180f).ToString("F2");
			meanDirPhi.text = Random.Range(0f, 360f).ToString("F2");
			SetMeanDirection();
		}
		if (randomizeGamma23.isOn)
		{
			axisAngle.text = Random.Range(0f, 360f).ToString("F2");
			SetAxis();
		}
		if (randomizeKappa.isOn)
		{
			kappa.text = Random.Range(0f, 500f).ToString("F2");
			SetKappa();
		}
		if (randomizeBeta.isOn)
		{
			beta.text = Random.Range(0f, randomKent.kappa/2).ToString("F2");
			SetBeta();
		}
		SetSampleNum();
		progressBar.gameObject.SetActive(true);
		randomKent.Generate();
	}

	/**
	 * \brief Aktualizacja ilości próbek.
	 *
	 * Aktualizuje docelową liczbę próbek.
	 */
	public void SetSampleNum()
	{
		int newNum = 1000;
		if (!string.IsNullOrEmpty(sampleNum.text))
			newNum = Mathf.Max(0, int.Parse(sampleNum.text));

		randomKent.numberOfSamples = newNum;
		CheckReady();
	}

	/**
	 * \brief Aktualizacja parametru koncentracji.
	 *
	 * Aktualizuje wartość parametru kappa klasy RandomKent.
	 * Upewnia się, że zależność 0 <= 2 * beta <= kappa jest zachowana.
	 */
	public void SetKappa()
	{
		float newKappa = 0;
		if (!string.IsNullOrEmpty(kappa.text))
			newKappa = float.Parse(kappa.text);

		if (newKappa < 2 * (randomKent.beta))
		{
			randomKent.beta = newKappa / 2;
			beta.text = (randomKent.beta).ToString("F2");
			SetBeta();
		}

		randomKent.kappa = newKappa;
		CheckReady();
	}

	/**
	 * \brief Aktualizacja parametru owalności.
	 *
	 * Aktualizuje wartość parametru beta klasy RandomKent.
	 * Upewnia się, że zależność 0 <= 2 * beta <= kappa jest zachowana.
	 */
	public void SetBeta()
	{
		float newBeta = 0;
		if (!string.IsNullOrEmpty(beta.text))
			newBeta = float.Parse(beta.text);

		if ((randomKent.kappa) < 2 * newBeta)
		{
			randomKent.kappa = newBeta * 2;
			kappa.text = (randomKent.kappa).ToString("F2");
			SetKappa();
		}

		randomKent.beta = newBeta;
		CheckReady();
	}

	/**
	 * \brief Aktualizacja osi wielkiej/małej.
	 *
	 * Aktualizuje wartość parametru gamma2 i gamma3 klasy RandomKent.
	 */
	public void SetAxis()
	{
		float angle = 0;
		if (!string.IsNullOrEmpty(axisAngle.text))
			angle = float.Parse(axisAngle.text);

		Vector3 output = Vector3.zero;
		Vector3.OrthoNormalize(ref randomKent.gamma1, ref output);
		randomKent.gamma2 = Quaternion.AngleAxis(angle, randomKent.gamma1) * output;
		randomKent.gamma3 = Quaternion.AngleAxis(angle + 90, randomKent.gamma1) * output;

		majorAxisLineRenderer.SetPosition(0, randomKent.gamma1 * 10 - randomKent.gamma2 * 2);
		majorAxisLineRenderer.SetPosition(1, randomKent.gamma1 * 10 + randomKent.gamma2 * 2);

		minorAxisLineRenderer.SetPosition(0, randomKent.gamma1 * 10 - randomKent.gamma3 * 1);
		minorAxisLineRenderer.SetPosition(1, randomKent.gamma1 * 10 + randomKent.gamma3 * 1);

		majorAxisX.text = randomKent.gamma2.x.ToString("F2");
		majorAxisY.text = randomKent.gamma2.z.ToString("F2");
		majorAxisZ.text = randomKent.gamma2.y.ToString("F2");

		minorAxisX.text = randomKent.gamma3.x.ToString("F2");
		minorAxisY.text = randomKent.gamma3.z.ToString("F2");
		minorAxisZ.text = randomKent.gamma3.y.ToString("F2");

		axisSetButton.image.color = Color.green;
		CheckReady();
	}

	/**
	 * \brief Aktualizacja średniego kierunku.
	 *
	 * Aktualizuje wartość parametru gamma1 klasy RandomKent.
	 */
	public void SetMeanDirection()
	{
		theta = 0;
		if (!string.IsNullOrEmpty(meanDirTheta.text))
			theta = float.Parse(meanDirTheta.text);
		if (theta >= 180)
		{
			theta = Mathf.PI;
			meanDirTheta.text = "180";
		}
		else if (theta <= 0)
		{
			theta = 0;
			meanDirTheta.text = "0";
		}
		else theta = theta * Mathf.Deg2Rad;

		float phi = 0;
		if (!string.IsNullOrEmpty(meanDirPhi.text))
			phi = float.Parse(meanDirPhi.text);
		if (phi >= 360)
		{
			phi = 2 * Mathf.PI;
			meanDirPhi.text = "360";
		}
		else if (phi <= 0)
		{
			phi = 0;
			meanDirPhi.text = "0";
		}
		else phi = phi * Mathf.Deg2Rad;

		float cosTheta = Mathf.Cos(theta);
		float x = Mathf.Sqrt(1 - Mathf.Pow(cosTheta, 2)) * Mathf.Cos(phi);
		float y = Mathf.Sqrt(1 - Mathf.Pow(cosTheta, 2)) * Mathf.Sin(phi);

		randomKent.gamma1 = new Vector3(x, cosTheta, y);
		meanDirLineRenderer.SetPosition(1, randomKent.gamma1 * 10);
		SetAxis();

		meanDirX.text = x.ToString("F2");
		meanDirY.text = y.ToString("F2");
		meanDirZ.text = cosTheta.ToString("F2");

		meanDirSetButton.image.color = Color.green;
		CheckReady();
	}

	/**
	 * \brief Otwarcie okna średniego kierunku.
	 *
	 * Aktywuje GameObject okna średniego kierunku.
	 */
	public void OpenMeanDirPopup()
	{
		meanDirPopUp.SetActive(true);
		meanDirLineRenderer.material = selected;
	}

	/**
	 * \brief Zamknięcie okna średniego kierunku.
	 *
	 * Dezaktywuje GameObject okna średniego kierunku.
	 */
	public void CloseMeanDirPopup()
	{
		meanDirPopUp.SetActive(false);
		meanDirLineRenderer.material = ambient;
	}

	/**
	 * \brief Otwarcie okna osi wielkiej/małej.
	 *
	 * Aktywuje GameObject okna osi wielkiej/małej.
	 */
	public void OpenAxisPopup()
	{
		axisPopUp.SetActive(true);
		minorAxisLineRenderer.material = selected;
		majorAxisLineRenderer.material = selected;
	}

	/**
	 * \brief Zamknięcie okna osi wielkiej/małej.
	 *
	 * Dezaktywuje GameObject okna osi wielkiej/małej.
	 */
	public void CloseAxisPopup()
	{
		axisPopUp.SetActive(false);
		minorAxisLineRenderer.material = ambient;
		majorAxisLineRenderer.material = ambient;
	}
}
