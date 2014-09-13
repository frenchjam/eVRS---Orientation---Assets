using UnityEngine;
using System.Collections;

// Use the keyboard to rotate an object in 3D.$
public class MoveByKeysMono : MonoBehaviour
{

	public float translateSpeed = 2f;
	public float rotateSpeed = 5f;

	public bool Shift = false;
	public bool Control = false;

	public enum DriveMode { rotate, translate };
	public DriveMode mode = DriveMode.rotate;

	public KeyCode up = KeyCode.UpArrow;
	public KeyCode down = KeyCode.DownArrow;
	public KeyCode left = KeyCode.LeftArrow;
	public KeyCode right = KeyCode.RightArrow;
	public KeyCode inward = KeyCode.Q;
	public KeyCode outward = KeyCode.S;

	public bool use_mouse = false;
	public float previous_mouse_x;
	public float previous_mouse_y;


	// These probably exist as globals or elements of some class, but I don't know where.
	// It was easier to define them here than to look any further.
	private Vector3 i_vector = new Vector3( 1.0f, 0.0f, 0.0f );
	private Vector3 j_vector = new Vector3( 0.0f, 1.0f, 0.0f );
	private Vector3 k_vector = new Vector3( 0.0f, 0.0f, 1.0f );

	private Vector3 startPosition = new Vector3( 0.0f, 0.0f, 0.0f );
	private Quaternion startRotation = new Quaternion( 0.0f, 0.0f, 0.0f, 1.0f );

	private bool stillDown = false;
	
	private float delta_x, delta_y;
	
	void Update ()
	{ 

		if ((Shift && !(Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))) ||
			(Control && !(Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)))) {

			// If the modifiers are not pressed as needed, just record the current mouse position for the next time through.
			previous_mouse_x = Input.mousePosition.x;
			previous_mouse_y = Input.mousePosition.y;
			return;

		}

		// Spacebar resets to starting view.
		if (Input.GetKey (KeyCode.Space)) {
				this.transform.rotation = startRotation;
				this.transform.position = startPosition;
		}
		if (Input.GetKey (KeyCode.D)) {
				if (mode == DriveMode.rotate && !stillDown) mode = DriveMode.translate;
				else if ( mode == DriveMode.translate && !stillDown) mode = DriveMode.rotate;
				stillDown = true;
		} else stillDown = false;

		// Update transform according to mode and which modifiers are pressed.
		// Alt key allows you to override to the other mode.
		if (mode == DriveMode.rotate || (mode == DriveMode.translate && (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)))) {
			if (Input.GetKey (left))  this.transform.Rotate (j_vector, - rotateSpeed * Time.deltaTime, Space.Self);
			if (Input.GetKey (right)) this.transform.Rotate (j_vector, rotateSpeed * Time.deltaTime, Space.Self);
			if (Input.GetKey (up)) this.transform.Rotate (i_vector, - rotateSpeed * Time.deltaTime, Space.Self);
			if (Input.GetKey (down)) this.transform.Rotate (i_vector, rotateSpeed * Time.deltaTime, Space.Self);	
			if (Input.GetKey (inward)) this.transform.Rotate (k_vector, rotateSpeed * Time.deltaTime, Space.Self);
			if (Input.GetKey (outward)) this.transform.Rotate (k_vector, - rotateSpeed * Time.deltaTime, Space.Self);
			if ( use_mouse ) {	
				delta_x = Input.mousePosition.x - previous_mouse_x;	
				this.transform.Rotate ( k_vector, delta_x / 50.0f, Space.Self );	
			}
		} else {
			if (Input.GetKey (left) ) this.transform.Translate (- translateSpeed * Time.deltaTime * i_vector, Space.Self);
			if (Input.GetKey (right) ) this.transform.Translate (translateSpeed * Time.deltaTime * i_vector, Space.Self);
			if (Input.GetKey (up)) this.transform.Translate (translateSpeed * Time.deltaTime * j_vector, Space.Self);
			if (Input.GetKey (down) ) this.transform.Translate (- translateSpeed * Time.deltaTime * j_vector, Space.Self);
			if (Input.GetKey (inward)) this.transform.Translate (- translateSpeed * Time.deltaTime * k_vector, Space.Self);
			if (Input.GetKey (outward) ) this.transform.Translate (translateSpeed * Time.deltaTime * k_vector, Space.Self);
			delta_x = Input.mousePosition.x - previous_mouse_x;	
			delta_y = Input.mousePosition.y - previous_mouse_y;	
			if ( use_mouse ) {	
				this.transform.Translate ( delta_x / 100.0f * i_vector, Space.Self );	
				this.transform.Translate ( delta_y / 100.0f * j_vector, Space.Self );
			}
		}
		
		// Save mouse position for next time.
		previous_mouse_x = Input.mousePosition.x;
		previous_mouse_y = Input.mousePosition.y;
		
	}

	void Start()
	{
		// Store the pose at onset, to be used to reset later.
		startRotation = this.transform.rotation;
		startPosition = this.transform.position;
		
		// Initialize the mouse position, so that we can calculate deltas later.
		previous_mouse_x = Input.mousePosition.x;
		previous_mouse_y = Input.mousePosition.y;
		
	}
}
