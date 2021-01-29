//Written by Justin Ortiz

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles information pertaining to each piece of content (i.e. Car Parts).
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshCollider))]
public class Content : MonoBehaviour {
	private static string shader_default = "Standard";
	private static string shader_lit = "Outline/Buffer";

	[SerializeField] private string content_name; //the name to display on the content label
	[SerializeField] private string content_description; //the description to display on the content label
	[SerializeField] private Vector3 custom_arrow_position; //a custom position to set the arrow to if this content is selected
	private Button button_selection; //the ui button for selecting this content
	private MeshRenderer mr;
	private Color color_original; //the original material color

	public string Name { get { return content_name; } }
	public string Description { get { return content_description; } }
	public Vector3 CustomArrowPosition { get { return custom_arrow_position; } }

	private void Awake() {
		if (string.IsNullOrEmpty(content_name)) //if a custom name was not given
			content_name = gameObject.name; //grab the game object name
	}

	private void OnDestroy() { //when this object is destroyed
		if (button_selection != null) { //if there is a ui button
			button_selection.onClick.RemoveAllListeners(); //remove the event listeners previously added via script
			Destroy(button_selection.gameObject); //also destroy its corresponding button
		}
	}

	private void OnMouseDown() { //when this object is clicked (collider required)
		if (!EventSystem.current.IsPointerOverGameObject()) //ensure the mouse is not on the UI
			Select(); //select this content
	}

	private void OnMouseEnter() { //first frame the mouse is hovering over this object
		if (!this.Equals(ContentManager.instance.Selected)) { //if this content is not the selected content
			if (!EventSystem.current.IsPointerOverGameObject()) { //ensure the mouse is not on the UI
				SetHighlight(true);
				mr.material.color = Color.cyan;
			}
		}
	}

	private void OnMouseExit() { //last frame the mouse is hovering over this object
		if (!this.Equals(ContentManager.instance.Selected)) { //if this content is not the selected content
			SetHighlight(false); //remove the highlight
		}
	}

	private void Select() {
		ContentManager.instance.Select(this); //have the manager select this content and pass info to the camera
	}

	/// <summary>
	/// Sets the shader on the mesh renderer to either display a highlight or return it to standard.
	/// </summary>
	/// <param name="active">True if highlight is desired.</param>
	public void SetHighlight(bool active) {
		string shader_desired = active ? shader_lit : shader_default; //update shader name based on active
		
		mr.material.shader = Shader.Find(shader_desired); //update the renderer
		mr.material.color = color_original; //reset the color
	}

	private void Start() {
		mr = GetComponent<MeshRenderer>();
		color_original = mr.material.color;
		button_selection = ContentManager.instance.InstantiateButton(content_name); //instantiate a button in the screen ui
		button_selection.onClick.AddListener(Select); //ensure when the button is clicked
	}
}
