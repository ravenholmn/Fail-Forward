using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeControl : MonoBehaviour
{
    public Image image;

    public void Fade()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float t = 0;
        float dur = 1;
        Color color = image.color;
        float startAlpha = color.a;

        yield return new WaitForSeconds(startAlpha == 1 ? 0.25f : 0);

        while (t < dur)
        {
            t += Time.deltaTime;

            color.a = startAlpha == 1 ? 1 - t : t;
            image.color = color;

            yield return null;
        }
    }
}
