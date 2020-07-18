using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColorScript : MonoBehaviour {
	public Color baseColor;
	public Color[] altColors;

	float hueThreshold = 0.1f;

	// TODO: pre-render and cache textures in each colour, and assign them to new car instances.
	void Start() {
		float bh, bs, bv;
		Color.RGBToHSV(baseColor, out bh, out bs, out bv);

		// Pick a random alt color; include -1 as an option to keep the base color.
		int index = Random.Range(0, altColors.Length);
		if (index == -1) {
			return;
		}
		Color newColor = altColors[index];
		float nh, ns, nv;
		Color.RGBToHSV(newColor, out nh, out ns, out nv);

		// Get the properties of this sprite renderer's instance of the material.
		SpriteRenderer spriter = GetComponent<SpriteRenderer>();
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		spriter.GetPropertyBlock(block);

		// Make a copy of the texture, so we don't alter the original asset permanently.
		Texture2D mainTex = (Texture2D)block.GetTexture("_MainTex");
		Texture2D newTex = Instantiate<Texture2D>(mainTex);

		// Apparently we can't just set _MainTex or mainTexture on the material directly.
		block.SetTexture("_MainTex", newTex);
		spriter.SetPropertyBlock(block);

		Color[] pixels = newTex.GetPixels();
		for (int i = 0; i < pixels.Length; i++) {
			if (pixels[i].a > 0) {
				float h, s, v;
				Color.RGBToHSV(pixels[i], out h, out s, out v);

				if (s > 0) {
					float diff = Mathf.Abs(bh - h);
					if (diff > .5f) {
						diff = 1 - diff;
					}

					if (diff < hueThreshold) {
						Color newPixel = Color.HSVToRGB(nh, ns, nv * v);
						newPixel.a = pixels[i].a;
						pixels[i] = newPixel;
					}
				}
			}
		}
		newTex.SetPixels(pixels);
		newTex.Apply(false);
	}
}
