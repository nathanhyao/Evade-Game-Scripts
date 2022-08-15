using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI = default;
    [SerializeField] private GameObject deathMenuUI = default;

    [SerializeField] private Toggle fogToggle = default;

    [SerializeField] private HeadBobController playerHeadBobScript = default;
    [SerializeField] private EnvironmentManager environmentManagerScript = default;

    void Start()
    {
        fogToggle.isOn = EnvironmentManager.useFog;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (!PlayerCollision.isDead)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        playerHeadBobScript.enabled = true;

        if (PlayerCollision.isDead)
            deathMenuUI.SetActive(true);

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerHeadBobScript.enabled = false;

        if (PlayerCollision.isDead)
            deathMenuUI.SetActive(false);

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        gameIsPaused = true;
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level...");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading menu...");
        // Time.timeScale = 1.0f;
        // SceneManager.LoadScene();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void ToggleFog(bool useFog)
    {
        Debug.Log("Environment Fog " + useFog);
        if (useFog)
            environmentManagerScript.FogOn();
        else
            environmentManagerScript.FogOff();
    }
}
