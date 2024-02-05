using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief klasa PointColorChanger
 *
 * klasa zmieniająca kolor puntków znajdujących się z tyłu sfery względem widoku z kamery.
 *
 * \version wersja 1.0
 */
public class PointColorChanger : MonoBehaviour
{
	public Material frontMaterial; /**< kolor kiedy punkt jest z przodu */
	public Material backMaterial; /**< kolor kiedy punkt jest z tyłu */
	public Renderer renderer; /**< komponent Renderer */

	/**
	 * \brief Wykrywa wejście w kolizję.
	 *
	 * Wykrywa wejście w kolizję z obiektem zajmującym "tylnią" część widoku, zmienia kolor punktu.
	 */
	private void OnTriggerEnter(Collider other)
	{
		renderer.material = backMaterial;
	}

	/**
	 * \brief Wykrywa opuszczenie kolizji.
	 *
	 * Wykrywa opuszczenie kolizji z obiektem zajmującym "tylnią" część widoku, zmienia kolor punktu.
	 */
	private void OnTriggerExit(Collider other)
	{
		renderer.material = frontMaterial;
	}
}
