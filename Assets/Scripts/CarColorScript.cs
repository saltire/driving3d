using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColorScript : MonoBehaviour {
	public Color baseColor;
	public Color[] altColors;

	void Start() {
		// Pick a random alt color; include -1 as an option to keep the base color.
		int index = Random.Range(-1, altColors.Length);
		if (index == -1) {
			return;
		}
		Color newColor = altColors[index];

		SpriteRenderer spriter = GetComponent<SpriteRenderer>();
		// Make a copy of the texture, so we don't alter the original asset permanently.
		Texture2D tex = (Texture2D)Instantiate(spriter.material.GetTexture("_MainTex"));

		// Apparently we can't just set _MainTex or mainTexture on the material directly.
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		block.SetTexture("_MainTex", tex);
		spriter.SetPropertyBlock(block);

		Color[] pixels = tex.GetPixels();
		for (int i = 0; i < pixels.Length; i++) {
			if (pixels[i] == baseColor) {
				pixels[i] = newColor;
			}
		}
		tex.SetPixels(pixels);
		tex.Apply(false);
	}
}
