using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BodiesMenu : MonoBehaviour
{
	public GameObject template;
	public Transform listObject;
	public List<GameObject> bodyObjects;

	private void Awake()
	{
		bodyObjects = new();
		template.SetActive(false);
	}

	private void OnEnable()
	{
		RefreshList();
	}

	public void RefreshList()
	{
		foreach (var obj in bodyObjects)
		{
			Destroy(obj);
		}
		bodyObjects.Clear();

		var bodies = FindObjectsOfType<CelestialBody>();
		foreach (var b in bodies)
		{
			GameObject obj = Instantiate(template, listObject);
			obj.name = b.name;
			obj.transform.GetChild(0).GetComponent<Image>().sprite = b.Icon;
			obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = b.Name;
			obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = b.Description;
			obj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<CameraController>().SetAnchorBody(b));
			obj.SetActive(true);

			bodyObjects.Add(obj);
		}
	}

	public void CloseMenu()
	{
		gameObject.SetActive(false);
	}
}