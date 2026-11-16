
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private static SceneLoader instance;
    private bool isTransitioning = false;

    void Awake()
    {
        // Prevent duplicate SceneLoaders
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Keep SceneLoader and its children between scenes
        DontDestroyOnLoad(gameObject);

        // Start with a black screen
        SetAlpha(1f);

        // Block clicks during initial fade
        if (fadeImage != null)
            fadeImage.raycastTarget = true;

        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionToScene(sceneName));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        // Block mouse interactions during transition
        if (fadeImage != null)
            fadeImage.raycastTarget = true;

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Load new scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade back from black
        yield return StartCoroutine(Fade(1f, 0f));

        // Allow mouse interactions again
        if (fadeImage != null)
            fadeImage.raycastTarget = false;

        isTransitioning = false;
    }

    IEnumerator FadeIn()
    {
        isTransitioning = true;

        // Block clicks while fading in
        if (fadeImage != null)
            fadeImage.raycastTarget = true;

        yield return StartCoroutine(Fade(1f, 0f));

        // Allow mouse interactions
        if (fadeImage != null)
            fadeImage.raycastTarget = false;

        isTransitioning = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(fadeDuration, 0.001f)
            );

            SetAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            yield return null;
        }

        SetAlpha(endAlpha);
    }

    void SetAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
