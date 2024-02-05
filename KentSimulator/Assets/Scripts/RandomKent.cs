using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief klasa RandomKent
 *
 * klasa zajmująca się próbkowaniem rozkładu kenta.
 *
 * \version wersja 1.0
 */
public class RandomKent : MonoBehaviour
{
	public Vector3 gamma1 = new Vector3(0, 0, 1); /**< wektor reprezentujący średni kierunek */
	public Vector3 gamma2 = new Vector3(1, 0, 0); /**< wektor reprezentujący oś wielką */
	public Vector3 gamma3 = new Vector3(0, 1, 0); /**< wektor reprezentujący oś małą */

	public float kappa; /**< parametr koncentracji */
	public float beta; /**< parametr owalności */
	public int numberOfSamples; /**< docelowa liczba próbek */
	public float precision = 0.0001f; /**< precyzja wyznaczania maksima funkcji */

	public GameObject pointParent; /**< GameObject, który po wygenerowaniu będzie rodzicem GameObjectów wszystkich punktów */
	public GameObject point; /**< prefab punktu */
	public GameObject[] points; /**< tablica wygenerowanych punktów */

	private float a; /**< zmienna pomocnicza do generacji rozkładu */
	private float b; /**< zmienna pomocnicza do generacji rozkładu */
	private float gamma; /**< zmienna pomocnicza do generacji rozkładu */
	private float lambda1; /**< zmienna pomocnicza do generacji rozkładu */
	private float lambda2; /**< zmienna pomocnicza do generacji rozkładu */
	private float c2; /**< zmienna pomocnicza do generacji rozkładu */

	private bool generated = false; /**< zmienna przechowująca informację czy wygenerowano już rozkład */
	public int i = 0; /**< numer aktualnie generowanej próbki */

	/**
	 * \brief Rozpoczyna proces generacji próbek.
	 *
	 * Wywołanie skutkuje przerwanie ewentualnego poprzedniego generowania, reset punktów, zaktualizowanie wartości parametrów pomocniczych oraz rozpoczęcie generacji.
	 * Jeśli kappa jest 0 rozpoczyna generację rozkładu Jednorodnego, w innym przypadku rozkładu Kenta
	 */
	public void Generate()
	{
		StopAllCoroutines();
		if (generated) DestroyPoints();
		CalculateParams();
		points = new GameObject[numberOfSamples];
		if(kappa == 0) StartCoroutine(GenerateUniformCorutine());
		else StartCoroutine(GenerateKentCorutine());
	}

	/**
	 * \brief Generuje próbki rozkładu Jednorodnego na sferze.
	 *
	 * Generuje docelową liczbę próbek z rozkładu Jenorodnego na sferze.
	 * Stopniowo iteruje wartość i.
	 * Wywołuje tworzenie instancji wygenerowanych punktów.
	 */
	IEnumerator GenerateUniformCorutine()
	{
		for (i = 0; i < numberOfSamples; i++)
		{
			yield return null;
			float n1 = GenerateNormal();
			float n2 = GenerateNormal();
			float n3 = GenerateNormal();
			float n = Mathf.Sqrt(n1 * n1 + n2 * n2 + n3 * n3);

			AddPoint(new Vector3(n1 / n, n2 / n, n3 / n).normalized, i);
		}
		generated = true;
	}

	/**
	 * \brief Generuje próbkę rozkładu Normalnego.
	 *
	 * Zwraca próbkę rozkładu Normalnego(0, 1).
	 * \return próbka rozkładu Normalnego(0, 1).
	 */
	private float GenerateNormal()
	{
		float u = Random.Range(0f, 1f);
		float v = Random.Range(0f, 1f);
		float x = 1.71553f * (u - 0.5f) / v;
		if (x * x <= 4 * (1 - u) || x * x <= -4 * Mathf.Log(u))
			return x;
		else
			return GenerateNormal();
	}

	/**
	 * \brief Generuje próbki rozkładu Kenta na sferze.
	 *
	 * Generuje docelową liczbę próbek z rozkładu Kenta na sferze.
	 * Stopniowo iteruje wartość i.
	 * Wywołuje tworzenie instancji wygenerowanych punktów.
	 */
	IEnumerator GenerateKentCorutine()
	{
		float maxU1 = CheckR1(FindMaxU1(0, 1));
		float maxU2 = CheckR2(FindMaxU2(0, 1));
		for (i = 0; i < numberOfSamples; i++)
		{
			float u1 = Random.Range(0f, maxU1);
			float u2 = Random.Range(0f, maxU2);

			float r1;
			float r2;
			float r;

			do
			{
				yield return null;
				r1 = GenerateExponential(lambda1);
				r2 = GenerateExponential(lambda2);
				r = r1 * r1 + r2 * r2;
			} while (r >= 1 || !AcceptR1(u1, r1) || !AcceptR2(u2, r2));

			r1 = Random.Range(0f, 1f) < 0.5 ? r1 : -r1;
			r2 = Random.Range(0f, 1f) < 0.5 ? r2 : -r2;
			
			AddPoint(CalculateVector(r1, r2, Mathf.Sqrt(r)), i);
		}
		pointParent.transform.rotation = Quaternion.LookRotation(gamma3, gamma1);
		generated = true;
	}

	/**
	 * \brief Szukanie maksymalnej wartości U1.
	 *
	 * Oblicza maksymalną wartość U1, dla której istnieje rozwiązanie.
	 * Działa na zasadzie rekurencji.
	 * \param[float] min minimalna sprawdzana wartość.
	 * \param[float] max maksymalna sprawdzana wartość.
	 * \return maksymalna wartości U1.
	 */
	private float FindMaxU1(float min, float max)
	{
		float between = max - min;
		float resultMin = CheckR1(min);
		float resultMax = CheckR1(max);
		if (between <= precision) return Mathf.Max(resultMin, resultMax);
		if (resultMin > resultMax) return FindMaxU1(min, min + between / 2f);
		else return FindMaxU1(min + between / 2f, max);
	}

	/**
	 * \brief Szukanie maksymalnej wartości U2.
	 *
	 * Oblicza maksymalną wartość U2, dla której istnieje rozwiązanie.
	 * Działa na zasadzie rekurencji.
	 * \param[float] min minimalna sprawdzana wartość.
	 * \param[float] max maksymalna sprawdzana wartość.
	 * \return maksymalna wartość U2.
	 */
	private float FindMaxU2(float min, float max)
	{
		float between = max - min;
		float resultMin = CheckR2(min);
		float resultMax = CheckR2(max);
		if (between <= precision) return Mathf.Max(resultMin, resultMax);
		if (resultMin > resultMax) return FindMaxU2(min, min + between / 2f);
		else return FindMaxU2(min + between / 2f, max);
	}

	/**
	 * \brief Niszczenie punktów.
	 *
	 * Niszczy wszystkie poprzednio wygenerowane próbki.
	 */
	private void DestroyPoints()
	{
		foreach (GameObject p in points)
			Destroy(p);
		generated = false;
		points = null;
		pointParent.transform.rotation = Quaternion.identity;
	}

	/**
	 * \brief Sprawdzenie akceptacji R1.
	 *
	 * Akceptuje lub odrzuca wartość R1.
	 * \param[float] u1 wartość do porówania.
	 * \param[float] r1 sprawdzana wartość.
	 * \return true jeśli można zaakceptować R1.
	 * \return false jeśli nie można zaakceptować R1.
	 */
	private bool AcceptR1(float u1, float r1)
	{
		return u1 <= CheckR1(r1);
	}

	/**
	 * \brief Obliczenia do akceptacji R1.
	 *
	 * Zwraca wynik obliczeń do akceptuji lub odrzucenia wartości R1.
	 * \param[float] r1 sprawdzana wartość.
	 * \return wynik obliczeń
	 */
	private float CheckR1(float r1)
	{
		return Mathf.Exp(-(a * Mathf.Pow(r1, 2) + lambda1 * Mathf.Pow(r1, 4)) / 2 + lambda1 * r1 - 1);
	}

	/**
	 * \brief Sprawdzenie akceptacji R2.
	 *
	 * Akceptuje lub odrzuca wartośćR2.
	 * \param[float] u2 wartość do porówania.
	 * \param[float] r2 sprawdzana wartość.
	 * \return true jeśli można zaakceptować R2.
	 * \return false jeśli nie można zaakceptować R2.
	 */
	private bool AcceptR2(float u2, float r2)
	{
		return u2 <= CheckR2(r2);
	}

	/**
	 * \brief Obliczenia do akceptacji R2.
	 *
	 * Zwraca wynik obliczeń do akceptuji lub odrzucenia wartości R2.
	 * \param[float] r2 sprawdzana wartość.
	 * \return wynik obliczeń
	 */
	private float CheckR2(float r2)
	{
		return Mathf.Exp(-(b * Mathf.Pow(r2, 2) - gamma * Mathf.Pow(r2, 4)) / 2 + lambda2 * r2 - c2);
	}

	/**
	 * \brief Generuje próbkę rozkładu Wykładniczego.
	 *
	 * Zwraca próbkę rozkładu Wykładniczego.
	 * \param[float] rate parametr skali .
	 * \return próbka rozkładu Wykładniczego.
	 */
	private float GenerateExponential(float rate)
	{
		float x = Random.Range(0f, 1f);
		return -1 / rate * Mathf.Log(1 - x);
	}

	/**
	 * \brief Aktualizacja parametrów.
	 *
	 * Aktualizuje wartości parametrów pomocniczych a, b, gamma, lambda1, lambda2, c2.
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

	/**
	 * \brief Tworzy nowy punkt.
	 *
	 * Tworzy nowy punkt na sferze na podstawie podanej pozycji. Dodaje jego numer do nazwy.
	 * \param[Vector3] position pozycja punktu.
	 * \param[int] num numer punktu.
	 */
	public void AddPoint(Vector3 position, int num)
	{
		GameObject newPoint = Instantiate(point, pointParent.transform);
		newPoint.name = "Point " + num;
		newPoint.transform.position = position * 10;
		points[num] = newPoint;
	}

	/**
	 * \brief Oblicza współrzędne w 3D.
	 *
	 * Konwertuje współrzędne na projektcji sfery na wektor 3D.
	 * \param[float] x1 pozycja na osi x na projekcji.
	 * \param[float] x2 pozycja na osi y na projekcji.
	 * \param[float] r parametr pomocniczy do oblcizeń.
	 * \return pozycja w 3D.
	 */
	private Vector3 CalculateVector(float x1, float x2, float r)
	{
		float x = Mathf.Sqrt(4 * (1 - r * r) * x1 * x1);
		if (x1 < 0) x = -x;

		float y = Mathf.Sqrt(4 * (1 - r * r) * x2 * x2);
		if (x2 < 0) y = -y;

		float z = 1 - 2 * r * r;

		return  new Vector3(x, z, y);
	}
}
