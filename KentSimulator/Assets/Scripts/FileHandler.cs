using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * \brief klasa FileHandler
 *
 * Klasa obsługująca generację pliku tekstowego.
 * Zapisuje wyniki genereacji do pliku ".txt".
 *
 * \version wersja 1.0
 */
public class FileHandler : MonoBehaviour
{
	public RandomKent kent; /**< klasa RandomKent */
	private int fileNumber = 0; /**< Numer pliku txt */

	/**
	 * \brief Zapisanie do pliku.
	 *
	 * Zapisuje wyniki genereacji do pliku "[fileName].txt" w folderze programu.
	 * Zwiększa numer pliku txt.
	 * \param[string] fileName nazwa pliku.
	 */
	public void Export(string fileName)
	{

		string filePath = fileName + "_" + fileNumber + ".txt";
		fileNumber++;

		string contents = "";

		contents += "kappa = " + kent.kappa + "\n";
		contents += "beta = " + kent.beta + "\n";
		contents += "gamma1 = " + kent.gamma1.x + " " + kent.gamma1.z + " " + kent.gamma1.y + "\n";
		contents += "gamma2 = " + kent.gamma2.x + " " + kent.gamma2.z + " " + kent.gamma2.y + "\n";
		contents += "gamma3 = " + kent.gamma3.x + " " + kent.gamma3.z + " " + kent.gamma3.y + "\n";
		contents += "\npoints: \n";

		File.WriteAllText(filePath, contents);

		int numberOfPoints = kent.points.Length;
		for (int i = 0; i < numberOfPoints; i++)
		{
			Vector3 coordinates = kent.points[i].transform.position/10;
			contents = coordinates.x + " " + coordinates.z + " " + coordinates.y + "\n";
			File.AppendAllText(filePath, contents);
		}
	}
}
