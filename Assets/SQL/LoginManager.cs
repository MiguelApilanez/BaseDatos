using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject signUpPanel;

    [Header("Login Fields")]
    public TMP_InputField loginUserField;
    public TMP_InputField loginPasswordField;

    [Header("SignUp Fields")]
    public TMP_InputField signUpUserField;
    public TMP_InputField signUpPasswordField;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    private string baseUrl = "http://localhost/juego/";

    public void TogglePanels(bool showSignUp)
    {
        signUpPanel.SetActive(showSignUp);
        loginPanel.SetActive(!showSignUp);
        feedbackText.text = "";
    }

    // Botón: Iniciar sesión
    public void Login()
    {
        StartCoroutine(LoginRequest());
    }

    // Botón: Crear usuario
    public void Register()
    {
        StartCoroutine(RegisterRequest());
    }

    IEnumerator LoginRequest()
    {
        WWWForm form = new WWWForm();
        form.AddField("user", loginUserField.text);
        form.AddField("password", loginPasswordField.text);

        UnityWebRequest www = UnityWebRequest.Post(baseUrl + "login.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Error de conexión: " + www.error;
        }
        else
        {
            string response = www.downloadHandler.text;
            if (response.StartsWith("success:"))
            {
                string username = response.Split(':')[1];
                feedbackText.text = "Bienvenido, " + username;
                yield return new WaitForSeconds(1.5f);
                SceneManager.LoadScene("GameScene");
            }
            else if (response == "wrong_password")
                feedbackText.text = "Contraseña incorrecta.";
            else if (response == "not_found")
                feedbackText.text = "Usuario no encontrado.";
            else
                feedbackText.text = "Error: " + response;
        }
    }
    IEnumerator RegisterRequest()
    {
        Debug.Log("Iniciando solicitud de registro...");
        WWWForm form = new WWWForm();
        form.AddField("email", signUpUserField.text);
        form.AddField("password", signUpPasswordField.text);

        UnityWebRequest www = UnityWebRequest.Post(baseUrl + "registro.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Error de conexión: " + www.error;
        }
        else
        {
            string response = www.downloadHandler.text;
            if (response == "success")
            {
                feedbackText.text = "Cuenta creada correctamente.";
                yield return new WaitForSeconds(1.5f);
                TogglePanels(false);
            }
            else if (response == "duplicate")
            {
                feedbackText.text = "Este correo ya está registrado.";
            }
            else if (response == "missing_data")
            {
                feedbackText.text = "Faltan datos. Revisa los campos.";
            }
            else
            {
                feedbackText.text = "Error del servidor: " + response;
            }
        }
    }
}