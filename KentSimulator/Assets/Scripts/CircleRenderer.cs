using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief klasa CircleRenderer
 *
 * Klasa rysująca tło projekcji.
 *
 * \version wersja 1.0
 */
public class CircleRenderer : MonoBehaviour
{
	public LineRenderer renderer; /**< Renderer */
	public int steps; /**< Z ilu prostych składa się przyblizenie okręgu */
	public float radius; /**< Promień okręgu */

	public GameObject lineHelper; /**< GameObject */
	public int helperCount; /**< Ilość dodatkowych linii w tle projekcji */
	public float helperLength; /**< Długoćć dodatkowych linii w tle projekcji */

	private Vector3[] helperBig; /**< Tabela punktów dodatkowych linii w tle projekcji na zewn�trznej cz�ci okr�gu */
	private Vector3[] helperSmall; /**< Tabela punktów dodatkowych linii w tle projekcji na wewn�trznej cz�ci okr�gu */

	/**
	 * \brief Wykonuje się na Starcie programu.
	 *
	 * Wykonuje się na Starcie programu.
	 * Rysuje tło do projekcji następnie wyłącza się.
	 */
	private void Start()
	{
		DrawCircle();
		DrawHelpers();
		this.gameObject.SetActive(false);
	}

	/**
	 * \brief Rysuje linie dodatkowe.
	 *
	 * Rysuje linie dodatkowe dookoła okręgu.
	 */
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

	/**
	 * \brief Rysuje okrąg.
	 *
	 * Rysuje okrąg dla tła projeckji.
	 */
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
