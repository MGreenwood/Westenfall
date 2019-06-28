using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginBehavior : MonoBehaviour
{
    [SerializeField]
    GameObject loginObject, newAccountObject;
    [SerializeField]
    Button login, createAccount;

    [SerializeField]
    Text username;
    [SerializeField]
    InputField password;


    [SerializeField] Text newUsername;
     [SerializeField] InputField newPassword, newPassword2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (loginObject.activeSelf)
                login.onClick.Invoke();
            else
                createAccount.onClick.Invoke();
                
        }
    }

    public void AttemptLogin()
    {
        string un = username.text.ToLower().Trim();

        // need to query for the user's salt here, null if first login, then store TODO
        byte[] salt = DatabaseManager.instance.GetUserSalt(un);
        byte[] pw = Encryption.HashValue(password.text.ToLower().Trim(), ref salt); // this will be salted and hashed

        Debug.Log($"Password: {BitConverter.ToString(pw).Replace("-", string.Empty)}   Salt: {BitConverter.ToString(salt).Replace("-", string.Empty)}");

        DatabaseManager.instance.Login(un, pw, LoginSuccessful);
    }

    public void Createaccount()
    {
        loginObject.SetActive(false);
        newAccountObject.SetActive(true);
    }

    public void Create()
    {
        // input validation
        if(newUsername.text == "" || newPassword.text == "" || newPassword2.text != newPassword.text)
        {
            return;
        }

        byte[] salt = null;
        byte[] hashedPassword = Encryption.HashValue(newPassword.text.Trim().ToLower(), ref salt);

        DatabaseManager.instance.CreateAccount(newUsername.text.Trim().ToLower(), hashedPassword, salt);
    }

    public void CancelAccountCreation()
    {
        loginObject.SetActive(true);
        newAccountObject.SetActive(false);
    }

    public void LoginSuccessful()
    {
        Debug.Log("Login Successful..."); // What does this mean? TODO
    }


    public static string ByteArrayToString(byte[] array)
    {
        string line = "";

        for (int i = 0; i < array.Length; i++)
        {
            line += $"{array[i]:X}";
        }

        return line;
    }
}
