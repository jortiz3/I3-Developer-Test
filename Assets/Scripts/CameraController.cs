using UnityEngine;

/// <summary>
/// Handles the rotation of the camera around a focus point.
/// 
/// File created by Justin Ortiz 1/28/2021
/// </summary>
public class CameraController : MonoBehaviour {
	public static CameraController instance; //the one instance of this class -- singleton pattern

	private Vector3 focus_previous; //the previous xyz coord the camera was focused on
	private Vector3 focus_current; //the current focus location for the camera
	private Vector3 focus_desired; //the transition target for the camera to focus on
	private Vector3 mousePosition_previous; //tracks the previous xy coordinates for the mouse
	private float zoom_previous; //tracks the previous zoom level for linear interpolation
	private float zoom_current; //the current zoom level; modified frequently
	private float zoom_desired; //tracks the target zoom level
	private float transition_percentage; //the percentage of completion for the linear interpolation
	private float rotation_angle; //the angle for the camera's rotation x
	private float rotation_speed; //the speed in which the user can rotate the camera

	private void Awake() {
		if (instance != null) { //if there is another instance
			Destroy(gameObject); //destroy this one
		} else { //else this is the only instance
			instance = this; //set reference to this
		}
	}

	/// <summary>
	/// Moves the focal point of the camera to a given object (or null)
	/// </summary>
	/// <param name="newFocus"></param>
	public void FocusOn(GameObject newFocus) {
		if (newFocus != null) { //if given an object
			focus_desired = newFocus.transform.position; //set the position
			zoom_desired = -3f; //zoom in closer is desired
		} else { //no object given
			focus_desired = Vector3.zero; //reset to the origin
			zoom_desired = -10f; //zoom out to view the whole car
		}

		focus_previous = focus_current; //set the current focus to the previous
		zoom_previous = zoom_current; //set the current zoom to the previous
		transition_percentage = 0f; //reset the progress of the linear interpolation
	}

	private void Start() {
		zoom_previous = -3f; //initialize various attributes
		zoom_current = -3f;
		zoom_desired = -10f;
		transition_percentage = 0f;
		rotation_angle = 0f;
		rotation_speed = 300f;
	}

	void Update() {
		if (Input.GetMouseButton(1)) { //each frame the user is holding right-click
			rotation_angle -= (mousePosition_previous - Camera.main.ScreenToViewportPoint(Input.mousePosition)).x * rotation_speed; //update the rotation (y) using the change in mouse position; modify by rotation speed
			transform.rotation = Quaternion.Euler(45, rotation_angle, 0); //set the new rotation
		}

		if (transition_percentage < 1) { //if there is a transition in progress
			transition_percentage = Mathf.Clamp(transition_percentage + Time.deltaTime, 0f, 1f); //increase the zoom percentage up to 1 and ensure it doesn't exceed by clamping
			zoom_current = Mathf.Lerp(zoom_previous, zoom_desired, transition_percentage); //update the current zoom level using linear interpolation
			focus_current = Vector3.Lerp(focus_previous, focus_desired, transition_percentage); //update the current focus point
		}

		transform.position = focus_current; //move to the focus position
		transform.Translate(new Vector3(0, 0, zoom_current)); //offset the camera so it is not within the focus point

		mousePosition_previous = Camera.main.ScreenToViewportPoint(Input.mousePosition); //always update the previous mouse position so the camera movement isn't sporadic
	}
}
