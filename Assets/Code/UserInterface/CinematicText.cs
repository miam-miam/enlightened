using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CinematicText : MonoBehaviour
{

    public Image[] blackBars;

    public TextMeshProUGUI majorText;

	public TextMeshProUGUI minorText;

    private float fadeInTime = 0;

    private float fadeTime = 0;

	// Update is called once per frame
	void Update()
    {
        float multiplier = Mathf.Clamp01(fadeInTime - fadeTime);
        fadeTime -= Time.deltaTime;
        majorText.alpha = Mathf.Clamp01(fadeTime) * multiplier;
		minorText.alpha = Mathf.Clamp01(fadeTime) * multiplier;
        foreach (var thing in blackBars)
            thing.color = new Color(0, 0, 0, Mathf.Clamp01(fadeTime) * multiplier);
	}

    public void DisplayText(string message, string minorMessage, float time)
    {
        majorText.text = message;
        minorText.text = minorMessage;
        fadeTime = time;
        fadeInTime = time;
	}

}
