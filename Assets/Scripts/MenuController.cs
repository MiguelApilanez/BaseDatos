using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject logrosPanel;
    public void StartButton()
    {
        SceneManager.LoadScene("GameScene");
        Debug.Log("Empieza el juego");
    }
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Se cierra el juego");
    }
    public void LogrosButton()
    {
        mainMenuPanel.SetActive(false);
        logrosPanel.SetActive(true);
    }
    public void LogrosBackButton()
    {
        mainMenuPanel.SetActive(true);
        logrosPanel.SetActive(false);
    }
    public void BackButton()
    {
        SceneManager.LoadScene("IndexScene");
    }
}
