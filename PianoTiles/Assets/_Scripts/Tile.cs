using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public Color activeColor;
	public GameManager manager;

	void Start () {
		manager = FindObjectOfType<GameManager> ();
		GetComponent<SpriteRenderer> ().color = activeColor;
	}
}
