using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gene : MonoBehaviour {

	public float points;
	public bool isAlive;

	public byte[] rgb = new byte[3];

	void Awake() {
		points = 0f;
		isAlive = true;
	}

	public void updateColor() {
		GetComponent<Image>().color = new Color32(rgb[0], rgb[1], rgb[2], 255);
	}

	public void setGene(byte r, byte g, byte b) {
		rgb[0] = r;
		rgb[1] = g;
		rgb[2] = b;
	}

	public void crossOver(Gene gene) {
		int random = Random.Range(0, 3);

		for (int i = 0; i <= random; i++) {
			rgb[i] = gene.rgb[i];
		}
	}

	public void mutation() {
		int random = Random.Range(0, 3);
		rgb[random] = (byte)Random.Range(1, 256);
	}
}
