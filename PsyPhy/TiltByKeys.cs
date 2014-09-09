using UnityEngine;
using System.Collections;

// Use the keyboard to rotate an object in 3D.
public class TiltByKeys : MonoBehaviour
{

	public float Speed = 5f;
	public bool Shift = false;
	public bool Control = false;
	public bool Alt = false;
	public bool Command = false;
	
	// These probably exist as globals or elements of some class, but I don't know where.
	// It was easier to define them here than to look any further.
	private Vector3 i_vector = new Vector3( 1.0f, 0.0f, 0.0f );
	private Vector3 j_vector = new Vector3( 0.0f, 1.0f, 0.0f );
	private Vector3 k_vector = new Vector3( 0.0f, 0.0f, 1.0f );
	private Quaternion null_rotation = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f );
	
	void Update ()
	{ 

		if ( Shift && !( Input.GetKey (KeyCode.LeftShift ) || Input.GetKey (KeyCode.RightShift) )) return;
		if ( Control && !( Input.GetKey (KeyCode.LeftControl ) || Input.GetKey (KeyCode.RightControl ) )) return;
		if ( Alt && !( Input.GetKey (KeyCode.LeftAlt ) || Input.GetKey (KeyCode.RightAlt ) )) return;
		if ( Command && !( Input.GetKey (KeyCode.LeftCommand ) || Input.GetKey (KeyCode.RightCommand ) )) return;

		// Spacebar resets to starting view.
		if (Input.GetKey (KeyCode.Space)) this.transform.rotation = null_rotation;

		// Look around to the left, right, up and down directions.
		if (Input.GetKey (KeyCode.LeftArrow))
		this.transform.Rotate (j_vector, - Speed * Time.deltaTime, Space.Self);
		if (Input.GetKey (KeyCode.RightArrow))
		this.transform.Rotate (j_vector, Speed * Time.deltaTime, Space.Self);
		if (Input.GetKey (KeyCode.UpArrow))
		this.transform.Rotate (i_vector, - Speed * Time.deltaTime, Space.Self);
		if (Input.GetKey (KeyCode.DownArrow))
		this.transform.Rotate (i_vector, Speed * Time.deltaTime, Space.Self);

		// Roll around the current heading.
		if (Input.GetKey (KeyCode.Q))
		this.transform.Rotate (k_vector, Speed * Time.deltaTime, Space.Self);
		if (Input.GetKey (KeyCode.S))
		this.transform.Rotate (k_vector, - Speed * Time.deltaTime, Space.Self);

	}
	
	void Start()
	{
	}
}
