using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;

    [SerializeField] private GameObject deathCanvas = null;
    [SerializeField] private Animator playerAnimator = null;

    bool isGameOver = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    //Is called when player dies
    public void PlayerDeath()
    {
        deathCanvas.SetActive(true);
        StartCoroutine(CanvasFade());

        isGameOver = true;

        playerAnimator.SetTrigger("Death");
    }

    //Fade in the death canvas
    private IEnumerator CanvasFade()
    {
        float elapsed = 0f;
        float time = 2;

        CanvasGroup canvasGroup = deathCanvas.GetComponent<CanvasGroup>();

        //Wait for 0.5 seconds before fading out the canvas & music
        yield return new WaitForSeconds(0.5f);

        SoundManager.instance.PlayMusic("shop", 0f, 2f, 0f);

        while (elapsed < time)
        {
            float currentAlpha = Mathf.Lerp(0f, 1f, elapsed / time);
            canvasGroup.alpha = currentAlpha;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        canvasGroup.alpha = 1f;
    }

    //Gameover Buttons
    public void TryAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
     /////////////////
  

    public bool GetGameOver
    {
        get { return isGameOver; }
    }
}