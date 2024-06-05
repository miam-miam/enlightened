using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HoldToRestart : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI restartingText;

    [SerializeField]
    public Image restartingImage;

    [SerializeField]
    public Image backgroundImage;

    private float heldTime = 0;

    private bool finished = false;

    private void Start()
    {
        restartingImage.enabled = true;
        restartingText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
            return;
        if (Input.GetKey(KeyCode.Escape))
        {
            heldTime += Time.deltaTime;
        }
        else
        {
            heldTime = Mathf.Max(0, heldTime - Time.deltaTime);
        }
        if (heldTime > 3)
        {
            StartCoroutine(Finish());
            return;
        }
        restartingImage.fillAmount = heldTime / 3f;
        restartingImage.color = new Color(1, 1, 1, Mathf.Min(1, heldTime / 0.5f));
        restartingText.color = new Color(1, 1, 1, Mathf.Min(1, heldTime / 0.5f));
        backgroundImage.color = new Color(0, 0, 0, Mathf.Min(1, heldTime / 0.5f));
    }

    private IEnumerator Finish()
    {
        finished = true;
        float scale = 1;
        FadeToBlack.FadeOut(0.25f, 0.25f);
        while (scale < 2)
        {
            scale += Time.deltaTime * 4;
            restartingImage.transform.localScale = new Vector3(scale, scale, 1);
            yield return new WaitForEndOfFrame();
        }
        heldTime = 0;
        finished = false;
        SceneManager.LoadScene("Introduction");
        restartingImage.transform.localScale = new Vector3(1, 1, 1);
        Transform t = GameObject.FindGameObjectWithTag("Player").transform;
        // Destroy the player once and for all
        Destroy(t.gameObject);
    }

}
