using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles information pertaining to each piece of content (i.e. Car Parts).
/// </summary>
/**
 * File created by Justin Ortiz 1/28/2021.
 */
[RequireComponent(typeof(MeshRenderer), typeof(MeshCollider))]
public class Content : MonoBehaviour {
	private static string shader_default = "Standard";
	private static string shader_lit = "Outline/Buffer";

	[SerializeField] private string content_name;
	[SerializeField] private string content_description;
	private Button button_selection;
	private MeshRenderer mr;

	public string Name { get { return content_name; } }
	public string Description { get { return content_description; } }

	private void Awake() {
		if (string.IsNullOrEmpty(content_name))
			content_name = gameObject.name;
	}

	private void OnDestroy() { //when this object is destroyed
		button_selection.onClick.RemoveAllListeners(); //remove the event listeners previously added via script
		Destroy(button_selection.gameObject); //also destroy its corresponding button
	}

	private void OnMouseDown() { //when this object is clicked (collider required)
		if (!EventSystem.current.IsPointerOverGameObject()) //ensure the mouse is not on the UI
			Select(); //select this content
	}

	private void Select() {
		ContentManager.instance.Select(this); //have the manager select this content and pass info to the camera
	}

	/// <summary>
	/// Sets the shader on the mesh renderer to either display a highlight or return it to standard.
	/// </summary>
	/// <param name="active">True if highlight is desired.</param>
	public void SetHighlight(bool active) {
		string shader_current = active ? shader_lit : shader_default; //determine which shader to obtain
		mr.material.shader = Shader.Find(shader_current); //update the renderer
	}

	private void Start() {
		mr = GetComponent<MeshRenderer>();
		button_selection = ContentManager.instance.InstantiateButton(content_name); //instantiate a button in the screen ui
		button_selection.onClick.AddListener(Select); //ensure when the button is clicked
	}
}
