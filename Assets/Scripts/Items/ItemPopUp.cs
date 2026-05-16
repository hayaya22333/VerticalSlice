using System.Collections;
using UnityEngine;

public class ItemPopUp : MonoBehaviour
{
    [SerializeField] float delay = 1f;
    [SerializeField] float fadeDuration = 0.5f;

    CanvasGroup canvasGroup;
    Coroutine fadeRoutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        fadeRoutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(delay);

        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }
}