using NohaSoftware.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeControl : MonoBehaviour
{
	public RectTransform timeControlPanel;
	public TextMeshProUGUI timeControlText;
	[Tooltip("Enter a float that controls the time scale")] public SerializableTuple<float, Button>[] timeControlButtons;
	public int currentlySelected;
	public bool enableTimeControl;

	// Start is called before the first frame update
	void Start()
	{
		if (!enableTimeControl || timeControlButtons == null) return;
		if (timeControlButtons.Length < 1)
		{
			Debug.LogWarning("No time control buttons added, playing at 1");
			gameObject.SetActive(false);
			return;
		}
		else
		{
			if (timeControlButtons.ContainsKey(1))	// try to find 1x time
			{
				for (int i = 0; i < timeControlButtons.Length; i++)
				{
					if (timeControlButtons[i].Key == 1f)
					{
						Debug.Log("Contains 1 at " + i);
						SelectButton(i);
						break;
					}
				}
			}
			else
			{
				SelectButton(0);
			}

			// setup button listeners
			for (int i = 0; i < timeControlButtons.Length; i++)
			{
				int ihateyou = i;
				timeControlButtons[i].Value.onClick.RemoveAllListeners();
				timeControlButtons[i].Value.onClick.AddListener(() => SelectButton(ihateyou));
			}
		}
		//Time.fixedDeltaTime = 0.1f;
	}

	void Update()
	{
		timeControlPanel.gameObject.SetActive(enableTimeControl);
	}

	void SelectButton(int index)
	{
		timeControlButtons[currentlySelected].Value.image.color = Color.white;
		currentlySelected = index;
		timeControlButtons[currentlySelected].Value.image.color = Color.yellow;

		float timeScale = timeControlButtons[currentlySelected].Key;

		Time.timeScale = timeScale;

		if (timeScale == 0) timeControlText.text = "Paused";
		else if (timeScale == 1) timeControlText.text = "";
		else timeControlText.text = timeScale + "x";
	}
}