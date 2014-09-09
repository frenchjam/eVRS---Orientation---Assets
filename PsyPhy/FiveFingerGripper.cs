using UnityEngine;
using System.Collections;

public class FiveFingerGripper : MonoBehaviour {

	public ConfigurableJoint[] joints;

	public Rigidbody[] fingers = new Rigidbody[5];
	public Vector3[] open = new Vector3[5];
	public Vector3[] closed = new Vector3[5];
	public Vector3[] current = new Vector3[5];

	private bool isStarted = false;

	// Use this for initialization
	void Start () {

		if (isStarted) return;

		joints = this.GetComponents<ConfigurableJoint> ();
		for (int i = 0; i < 5; i++) {
			// Store the corresponding rigid body.
			fingers [i] = joints [i].connectedBody;
			// Store the open position of each finger as the position
			//  when the game is started.
			open [i] = fingers [i].transform.position;
			// The closed position of each finger is set by 
			//  setting the x position to zero.
			closed [i] = open [i];
			closed [i].x = 0.0f;
		}
		isStarted = true;


	}

	// Update is called once per frame
	void Update () {
		// Make sure that everything is initialized.
		if (!isStarted) Start ();
		// Get the current target position for each joint, just for debugging.
		for (int i = 0; i < 5; i++) {
			current [i] = joints[i].targetPosition;
		}
	}

	public void Open () {
		// Make sure that everything is initialized.
		if (!isStarted) Start ();
		// Tell each joint to move towards its open position.
		for (int i = 0; i < 5; i++) {
			joints[i].targetPosition = open[i];
		}
	}

	public void Close () {
		// Make sure that everything is initialized.
		if (!isStarted) Start ();
		// Tell each joint to move towards its closed position.
		for (int i = 0; i < 5; i++) {
			joints[i].targetPosition =closed[i];
		}
	}


}
