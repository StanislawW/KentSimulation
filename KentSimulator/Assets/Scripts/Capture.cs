using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Capture : MonoBehaviour
{
	private int i = 0;
	public GameObject canvas;
	public GameObject props;
	public GameObject lines;

	public float screenshotTime;
	private float counter = -1;
	private Color camColor;

	private void Start()
	{
		camColor = Camera.main.backgroundColor;
	}

	private void Update()
	{
		if(counter > 0)
		{
			counter -= Time.deltaTime;
			if (counter < 0)
				EndCapture();
		}
		else if(Input.GetKeyDown(KeyCode.Q))
		{
			StartCapture();
		}
	}

	public void StartCapture()
	{
		canvas.SetActive(false);
		props.SetActive(false);
		lines.SetActive(true);
		Camera.main.backgroundColor = Color.white;
		CaptureScreen();
		counter = screenshotTime;
	}

	private void EndCapture()
	{
		lines.SetActive(false);
		canvas.SetActive(true);
		props.SetActive(true);
		Camera.main.backgroundColor = camColor;
	}

	private void CaptureScreen()
	{
		ScreenCapture.CaptureScreenshot("kent_" + i + ".png");
		i++;
	}
}
