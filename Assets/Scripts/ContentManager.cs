//Coded by Justin Ortiz

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visibility of content and content descriptions.
/// </summary>
public class ContentManager : MonoBehaviour {
	public static ContentManager instance; //reference to the one instance of this class -- singleton pattern

	[SerializeField] private Button template_button;
	[SerializeField] private Transform label_content;
	private Content selected; //reference to the Content object the user has selected

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
		if (template_button == null) {
			return null;
		}

		Button instantiated = Instantiate(template_button.gameObject, template_button.transform.parent).GetComponent<Button>(); //instantiate a copy of the prefab
		instantiated.transform.GetChild(0).GetComponent<Text>().text = !string.IsNullOrEmpty(text) ? text : "Content " + instantiated.transform.GetSiblingIndex();
		instantiated.gameObject.SetActive(true);
		return instantiated;
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

			CameraController.instance.FocusOn(selected.gameObject); //focus the camera on the selected content
		} else {
			label_content.gameObject.SetActive(false); //hide the label
			CameraController.instance.FocusOn(null); //focus the camera on null
		}
	}

	private void Start() {
		if (label_content != null) {
			label_content.gameObject.SetActive(false);
		}
	}
}
