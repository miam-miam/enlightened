using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UserInterfaceTimer : MonoBehaviour
{

    public DateTime timeElapsed;

    private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update()
    {
		timeElapsed = timeElapsed.AddSeconds(Time.deltaTime);
		text.text = $"{timeElapsed.ToString("m:ss:ff")}";
	}
}
