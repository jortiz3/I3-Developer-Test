//Written by Justin Ortiz

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the selection of content and the visibility of both the content label and the arrow.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ContentManager : MonoBehaviour {
	public static ContentManager instance; //reference to the one instance of this class -- singleton pattern

	[SerializeField] private Transform label_content; //the reusable label for all content
	[SerializeField] private Transform arrow_content; //the reusable arrow for all content
	[SerializeField] private Button template_button; //a template gameobject for buttons that is hidden within the scene -- to preserve the transform parent
	private Content selected; //reference to the Content object the user has selected

	public Content Selected { get { return selected; } }

	private void Awake() {
		if (instance != null) { //if another instance exists
			Destroy(gameObject); //destroy this one
		} else { //else this is the only instance
			instance = this; //set reference to this
		}
	}

	/// <summary>
	/// Instantiates a copy of the template button within the scene.
	/// </summary>
	/// <param name="text"></param>
	/// <returns>A copy of the button template.</returns>
	public Button InstantiateButton(string text = "") {
		if (template_button == null) { //if there is no template
			return null;
		}

		Button instantiated = Instantiate(template_button.gameObject, template_button.transform.parent).GetComponent<Button>(); //instantiate a copy of the prefab
		instantiated.transform.GetChild(0).GetComponent<Text>().text = !string.IsNullOrEmpty(text) ? text : "Content " + instantiated.transform.GetSiblingIndex(); //set the text or generate text
		instantiated.gameObject.SetActive(true); //display the button on the ui
		return instantiated; //return the instantiated button
	}

	/// <summary>
	/// Handles enabling and disabling content highlights as well as updating the camera focus.
	/// </summary>
	/// <param name="c">The content object the user has selected.</param>
	public void Select(Content c) {
		if (c != null) { //if the given content has value
			if (selected != null) //if the user previously selected content
				selected.SetHighlight(false); //remove the highlight

			if (!c.Equals(selected))//first time user selects or user selects a different part
				selected = c; //update the new selected object
			else //user selected the same object twice
				selected = null; //remove reference
		}

		if (selected != null) { //if there is content currently selected
			selected.SetHighlight(true); //highlight the content

			label_content.Find("Title").GetComponent<Text>().text = selected.Name; //update the title text
			label_content.Find("Description").GetComponent<Text>().text = selected.Description; //update the description text
			label_content.gameObject.SetActive(true); //set label to visible

			if (!selected.CustomArrowPosition.Equals(Vector3.zero)) { //if the user has set a custom arrow position
				arrow_content.position = selected.CustomArrowPosition; //use the custom position
			} else { //no custom position
				arrow_content.position = selected.transform.position * 1.7f; //automatically generate position
			}
			arrow_content.LookAt(selected.transform); //ensure the arrow is pointing at the selected object

			arrow_content.gameObject.SetActive(true); //show the arrow to the user

			CameraController.instance.FocusOn(selected.gameObject); //focus the camera on the selected content
		} else {
			label_content.gameObject.SetActive(false); //hide the label
			arrow_content.gameObject.SetActive(false); //hide the arrow
			CameraController.instance.FocusOn(null); //focus the camera on null
		}
	}

	private void Start() {
		if (label_content == null) //show a warning to the dev team if something is missing
			Debug.Log("Member 'label_content' is not assigned. Please assign a value using the inspector.");

		if (arrow_content == null)
			Debug.Log("Member 'arrow_content' is not assigned. Please assign a value using the inspector.");

		if (template_button == null)
			Debug.Log("Member 'template_button' is not assigned. Please assign a value using the inspector.");

		label_content.gameObject.SetActive(false); //hide the label
		arrow_content.gameObject.SetActive(false); //hide the arrow
	}
}
