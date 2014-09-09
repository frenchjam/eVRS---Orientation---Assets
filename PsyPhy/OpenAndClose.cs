using UnityEngine;
using System.Collections;

public class OpenAndClose : MonoBehaviour {

	private ConfigurableJoint[] fingers;

	private Vector3 openThumb = new Vector3( -0.1f, 0.0f, 0.0f );
	private Vector3 openFinger = new Vector3( 0.1f, 0.0f, 0.0f );
	private Vector3 closed = new Vector3( 0.0f, 0.0f, 0.0f );

	// Use this for initialization
	void Start () {
		fingers = this.GetComponents<ConfigurableJoint> ();
	}

	// Update is called once per frame
	void Update () {}

	public void Open () {
		fingers = this.GetComponents<ConfigurableJoint> ();
		fingers [0].targetPosition = openThumb;
		for (int i = 1; i < 5; i++)
						fingers [i].targetPosition = openFinger;
	}

	public void Close () {
		for (int i = 0; i < 5; i++)
			fingers [i].targetPosition = closed;
	}


}
