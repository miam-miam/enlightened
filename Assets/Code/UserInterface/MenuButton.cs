using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	private TextMeshProUGUI buttonText;

	private string ogText;

	// Start is called before the first frame update
	void Start()
    {
		buttonText = GetComponentInChildren<TextMeshProUGUI>();
		if (buttonText == null)
			Destroy(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ogText = buttonText.text;
		buttonText.text = $"\uf0da {buttonText.text}";
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		buttonText.text = ogText;
	}
}
