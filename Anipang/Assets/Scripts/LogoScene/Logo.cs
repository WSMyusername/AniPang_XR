using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    Image LogoImage;
    [SerializeField]
    bool isFadeIn, isFadeOut;

    private void Start()
    {
        if (LogoImage == null)
        {
            LogoImage = GameObject.Find("Logo").GetComponent<Image>();
        }

        LogoImage.color = new Color(LogoImage.color.r, LogoImage.color.g, LogoImage.color.b, 0);
        LogoImage.gameObject.SetActive(false);

        ImageFade(LogoImage);
    }

    void ImageFade(Image img)
    {
        img.gameObject.SetActive(true);

        isFadeIn = true;
        isFadeOut = false;

        StartCoroutine(FadeIn(img));
        StartCoroutine(FadeOut(img));
    }

    IEnumerator FadeIn(Image img)
    {
        while (isFadeIn == true)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.005f);
            yield return new WaitForSecondsRealtime(0.01f);

            if (img.color.a > 0.99f)
            {
                isFadeOut = true;
                yield return StartCoroutine(FadeOut(img));
                isFadeIn = false;
            }
        }
        
    }

    IEnumerator FadeOut(Image img)
    {
        while (isFadeOut == true)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - 0.005f);
            yield return new WaitForSecondsRealtime(0.01f);

            if (img.color.a < 0.01f)
            {
                SceneManager.LoadScene(1);
                isFadeOut = false;
            }

        }        
    }


}
