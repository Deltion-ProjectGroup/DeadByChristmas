using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour {
	[SerializeField] Material materialToScroll;
	[SerializeField] float uScrollSpeed;
	[SerializeField] float vScrollSpeed;

	float uOffset;
	float vOffset;

	void Update () {
		//Set offset
		uOffset += Time.deltaTime * uScrollSpeed;
		vOffset += Time.deltaTime * vScrollSpeed;

		//Apply offset to texture
		materialToScroll.SetTextureOffset("_MainTex", new Vector2(uOffset, vOffset));
	}
}
