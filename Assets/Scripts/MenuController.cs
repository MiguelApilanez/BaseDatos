using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
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
}
