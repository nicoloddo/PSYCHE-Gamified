using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    private ChoiceController cc;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        cc = FindObjectOfType<ChoiceController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartProgressBar(float time_amount)
    {
        gameObject.SetActive(true);
        StartCoroutine(DecreaseProgressBar(time_amount, false));
    }

    public void StartProgressBarChoice(float time_amount)
    {
        gameObject.SetActive(true);
        StartCoroutine(DecreaseProgressBar(time_amount, true));
    }

    public IEnumerator DecreaseProgressBar(float time_amount, bool send)
    {
        // Get reference to Image component
        Image progressBarImage = GetComponent<Image>();

        // Make sure progress bar is initially full
        progressBarImage.fillAmount = 1f;

        if (send)
        {
            cc.SendData("ChoiceTimeStarted()");
        }

        // Track how much time has passed
        float timePassed = 0;

        // Loop until enough time has passed
        while (timePassed < time_amount)
        {
            // Update time passed
            timePassed += Time.deltaTime;

            // Calculate new fill amount
            float fillAmount = Mathf.Lerp(1, 0, timePassed / time_amount);

            // Apply new fill amount to progress bar
            progressBarImage.fillAmount = fillAmount;

            // Wait until next frame
            yield return null;
        }

        // Make sure progress bar is empty at the end
        progressBarImage.fillAmount = 0;

        if (send)
        {
            cc.SendData("ChoiceTimeExpired()");
        }
        
        gameObject.SetActive(false);
    }

}
