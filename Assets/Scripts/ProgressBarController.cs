using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartProgressBar(float time_amount)
    {
        gameObject.SetActive(true);
        StartCoroutine(DecreaseProgressBar(time_amount));
    }

    public IEnumerator DecreaseProgressBar(float time_amount)
    {
        // Get reference to Image component
        Image progressBarImage = GetComponent<Image>();

        // Make sure progress bar is initially full
        progressBarImage.fillAmount = 1f;

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

        gameObject.SetActive(false);
    }

}
