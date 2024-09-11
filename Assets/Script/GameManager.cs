using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class TimerEvent : UnityEngine.Events.UnityEvent<float> { }

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public TimerEvent onTimerEvent = new TimerEvent();

    public static bool isStart = false;
    public static bool isPause;
    public static bool isClear;
    public static bool isGameOver;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject clearScreen;
    public GameObject textScore;
    public TextMeshProUGUI textTimer;

    private Gun gun;

    private float Timer;

    private void Awake()
    {
        isPause = false;
        isClear = false;
        isGameOver = false;
        Timer = 0;

        gun = GetComponent<Gun>();
        onTimerEvent.AddListener(UpdateTimerHUD);
        onTimerEvent.Invoke(Timer);
    }

    void Update()
    {
        if (isStart == true)
        {
            Timer += Time.deltaTime;
            onTimerEvent.Invoke(Timer);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
        // (GameObject.FindWithTag("ImpactEnemy") != null && gun.weaponSetting.currentMagazine <= 0 && gun.weaponSetting.currentAmmo <= 0)
        if (PlayerController.isDie) GameOver();
        if (GameObject.FindWithTag("ImpactEnemy") == null && EnemyMemoryPool.currentNumber == EnemyMemoryPool.maximumNumber && GameObject.FindWithTag("Player") != null) Clear();
    }

    public void UpdateTimerHUD(float current)
    {
        textTimer.text = $"Time : {string.Format("{0:N2}", current)}s";
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        textScore.SetActive(false);
        isPause = false;
        isStart = true;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        textScore.SetActive(false);
        isPause = false;
        isStart = true;
        PlayerController.isDie = false;
        SceneManager.LoadScene("PlayScene");
    }

    public void GameOver()
    {
        isGameOver = true;
        isStart = false;
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        textScore.SetActive(true);
    }

    void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        textScore.SetActive(true);
        isPause = true;
        isStart = false;
    }

    public void Clear()
    {
        isClear = true;
        isStart = false;
        Time.timeScale = 0f;
        clearScreen.SetActive(true);
        textScore.SetActive(true);
    }

    public void MainMenu()
    {
        isStart = false;
        Timer = 0f;
        Time.timeScale = 1f;
        PlayerController.isDie = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void GameStart()
    {
        isStart = true;
        SceneManager.LoadScene("PlayScene");
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
