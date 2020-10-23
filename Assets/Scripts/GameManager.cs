using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int totalGenesX;
	public int totalGenesY;
	public int offset;
	public GameObject cubePrefab;
	public Transform cubesParent;

	public Gene[,] genes;
	public Gene[] bestGenes;

	private int genesCount;

	void Awake() {
		genes = new Gene[totalGenesX, totalGenesY];
		bestGenes = new Gene[3];
		genesCount = 0;
	}

	void Start() {
		startGenes();
		bestGenes[0] = genes[0, 0];
		bestGenes[1] = genes[0, 1];
		bestGenes[2] = genes[0, 2];
	}

	private void startGenes() {
		for (int i = 0; i < totalGenesX; i++) {
			for (int j = 0; j < totalGenesY; j++) {
				GameObject go = Instantiate(cubePrefab, cubesParent);
				go.transform.localPosition = new Vector3(i * offset, j * offset, 0);

				int r = UnityEngine.Random.Range(0, 256);
				int g = UnityEngine.Random.Range(0, 256);
				int b = UnityEngine.Random.Range(0, 256);
				Gene gene = go.GetComponent<Gene>();
				gene.setGene((byte)r, (byte)g, (byte)b);

				genes[i, j] = gene;
				genes[i, j].updateColor();
			}
		}

		StartCoroutine(destroyGenes());
	}

	private void selectGenes() {
		int maxBestGenes = 3;

		for (int genesCount = 0; genesCount < maxBestGenes; genesCount++) {
			for (int i = 0; i < totalGenesX; i++) {
				for (int j = 0; j < totalGenesY; j++) {
					bool isGenesEquals = false;

					if (genesCount != 0) {
						for (int pastGenes = 1; pastGenes <= genesCount; pastGenes++) {
							isGenesEquals = genes[i, j] == bestGenes[genesCount - pastGenes];
						}
					}

					if (!isGenesEquals) {
						if (genes[i, j].points > bestGenes[genesCount].points) {
							bestGenes[genesCount] = genes[i, j];
						}
					}
				}
			}
		}
	}

	private void resetGenesPoints() {
		for (int i = 0; i < totalGenesX; i++) {
			for (int j = 0; j < totalGenesY; j++) {
				genes[i, j].points = 0;
			}
		}
	}

	private bool genesDidWin() {
		int winnerGenes = 0;

		for (int i = 0; i < totalGenesX; i++) {
			for (int j = 0; j < totalGenesY; j++) {
				if (
					genes[i, j].rgb[0] < 5 &&
					genes[i, j].rgb[1] < 5 &&
					genes[i, j].rgb[2] < 5
				) {
					winnerGenes++;
				}
			}
		}

		return winnerGenes == totalGenesX * totalGenesY ? true : false;
	}

	IEnumerator destroyGenes() {
		while (true) {
			yield return new WaitForSeconds(0.01f);

			for (int i = 0; i < totalGenesX; i++) {
				for (int j = 0; j < totalGenesY; j++) {
					if (genes[i, j].isAlive) {
						int random = UnityEngine.Random.Range(0, 101);

						if (random <= calculateChance(genes[i, j].rgb[0], genes[i, j].rgb[1], genes[i, j].rgb[2])) {
							genes[i, j].isAlive = false;
							genes[i, j].points = 1000f / (genes[i, j].rgb[0] + genes[i, j].rgb[1] + genes[i, j].rgb[2]) * Time.deltaTime;;
							genesCount++;
						}
					}
				}
			}

			if (genesCount == totalGenesX * totalGenesY) {
				if (!genesDidWin()) {
					restartGenes();
				} else {
					Debug.Log("Finally!");
				}
			}
		}
	}

	private void restartGenes() {
		genesCount = 0;

		selectGenes();

		for (int i = 0; i < totalGenesX; i++) {
			for (int j = 0; j < totalGenesY; j++) {
				if (genes[i, j] != bestGenes[0] || genes[i, j] != bestGenes[1] || genes[i, j] != bestGenes[2]) {
					int random = UnityEngine.Random.Range(0, 101);

					if (random < 97) {
						int randomBest = UnityEngine.Random.Range(0, 100);
						if (randomBest < 40) {
							genes[i, j].crossOver(bestGenes[0]);
						} else if (randomBest < 75) {
							genes[i, j].crossOver(bestGenes[1]);
						} else {
							genes[i, j].crossOver(bestGenes[2]);
						}
					} else {
						genes[i, j].mutation();
					}
				}

				genes[i, j].isAlive = true;
				genes[i, j].updateColor();
			}
		}
	}

	private int calculateChance(int r, int g, int b) {
		float rPercent = r * 100 / 255f;
		float gPercent = g * 100 / 255f;
		float bPercent = b * 100 / 255f;

		float totalChance = (r + g + b) / 3f;

		return (int)totalChance;
	}
}
