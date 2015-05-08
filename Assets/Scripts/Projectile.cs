using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public GameObject hitExplosion;
	public GameObject missExplosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.transform.tag == "Target") {
			Instantiate (hitExplosion, collision.transform.position, transform.rotation);
		} else {
			Instantiate (missExplosion, transform.position, transform.rotation);
		}
		Destroy (gameObject);
	}
}
