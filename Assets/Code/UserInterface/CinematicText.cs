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

    private float fadeTime = 0;

	// Update is called once per frame
	void Update()
    {
        fadeTime -= Time.deltaTime;
        majorText.alpha = Mathf.Clamp01(fadeTime);
		minorText.alpha = Mathf.Clamp01(fadeTime);
        foreach (var thing in blackBars)
            thing.color = new Color(1, 1, 1, Mathf.Clamp01(fadeTime));
	}

    public void DisplayText(string message, string minorMessage, float time)
    {
        majorText.text = message;
        minorText.text = minorMessage;
        fadeTime = time;
	}

}
