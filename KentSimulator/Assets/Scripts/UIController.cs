using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	[Header("General")]
	public RandomKent randomKent;
	public Toggle showGizmos;
	public Toggle showAxes;
	public GameObject axesLines;
	public Material selected;
	public Material ambient;
	public TMP_InputField sampleNum;
	public Button readyButton;
	public FileHandler fileHandler;
	public Capture capture;
	public GameObject exitPopUp;

	[Header("Camera Buttons")]
	public GameObject camHandle;
	public Button xButton;
	public Button yButton;
	public Button zButton;

	[Header("Random Toggles")]
	public Toggle randomizeGamma1;
	public Toggle randomizeGamma23;
	public Toggle randomizeKappa;
	public Toggle randomizeBeta;

	[Header("Kappa Parameters")]
	public TMP_InputField kappa;

	[Header("Beta Parameters")]
	public TMP_InputField beta;

	[Header("Mean Direction Parameters")]
	public GameObject meanDirPopUp;
	public TMP_InputField meanDirTheta;
	public TMP_InputField meanDirPhi;
	public TMP_Text meanDirX;
	public TMP_Text meanDirY;
	public TMP_Text meanDirZ;
	public Button meanDirSetButton;
	public LineRenderer meanDirLineRenderer;

	[Header("Axis Parameters")]
	public GameObject axisPopUp;
	public TMP_InputField axisAngle;
	public TMP_Text majorAxisX;
	public TMP_Text majorAxisY;
	public TMP_Text majorAxisZ;
	public TMP_Text minorAxisX;
	public TMP_Text minorAxisY;
	public TMP_Text minorAxisZ;
	public Button axisSetButton;
	public LineRenderer majorAxisLineRenderer;
	public LineRenderer minorAxisLineRenderer;

	private float theta = Mathf.PI/2;


	private void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			OpenExitPopUp();
		}
	}

	public void CloseExitPopUp()
	{
		exitPopUp.SetActive(false);
	}

	public void OpenExitPopUp()
	{
		exitPopUp.SetActive(true);
		CloseAxisPopup();
		CloseMeanDirPopup();
	}

	public void ExitApp()
	{
		Application.Quit();
	}

	public void ExportTxt()
	{
		fileHandler.Export("kent");
	}

	public void ExportImg()
	{
		capture.StartCapture();
	}

	public void XButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(-90, -90, 0);
	}
	public void YButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(-90, -180, 0);
	}
	public void ZButton()
	{
		camHandle.transform.rotation = Quaternion.Euler(0, 0, 0);
	}

	public void ToggleGizmos()
	{
		axesLines.SetActive(showAxes.isOn);
		meanDirLineRenderer.gameObject.SetActive(showGizmos.isOn);
		majorAxisLineRenderer.gameObject.SetActive(showGizmos.isOn);
		minorAxisLineRenderer.gameObject.SetActive(showGizmos.isOn);
	}

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
		randomKent.Generate();
	}

	public void SetSampleNum()
	{
		int newNum = 1000;
		if (!string.IsNullOrEmpty(sampleNum.text))
			newNum = Mathf.Max(0, int.Parse(sampleNum.text));

		randomKent.numberOfSamples = newNum;
		CheckReady();
	}

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

	public void OpenMeanDirPopup()
	{
		meanDirPopUp.SetActive(true);
		meanDirLineRenderer.material = selected;
	}

	public void CloseMeanDirPopup()
	{
		meanDirPopUp.SetActive(false);
		meanDirLineRenderer.material = ambient;
	}

	public void OpenAxisPopup()
	{
		axisPopUp.SetActive(true);
		minorAxisLineRenderer.material = selected;
		majorAxisLineRenderer.material = selected;
	}

	public void CloseAxisPopup()
	{
		axisPopUp.SetActive(false);
		minorAxisLineRenderer.material = ambient;
		majorAxisLineRenderer.material = ambient;
	}
}
