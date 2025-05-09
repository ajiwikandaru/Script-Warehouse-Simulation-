using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;

public class RfidLoginManager : MonoBehaviour
{
    //public GoogleSheetsConnector connector; // Reference to the Google Sheets connector
    private JArray accounts; // Array to store account data
    public TextMeshProUGUI usernameInputField; // Input field for username
    public TextMeshProUGUI passwordInputField; // Input field for password
    public GameObject loginPanel;
    public GameObject keyboard;
    public GameObject ErrorLoginPopUp;

    private string username; // Variable to store the username
    private string password; // Variable to store the password
    void Start()
    {
        /*StartCoroutine(connector.ReadData("Accounts", (data) =>
        {
            // Parse JSON response
            JObject parsedData = JObject.Parse(data);
            accounts = (JArray)parsedData["values"];
            accounts.RemoveAt(0);
        }));*/
    }

    // Check if the username and password matches
    public void CheckCredential()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        username = username.Substring(0, username.Length - 1);
        password = password.Substring(0, password.Length - 1);

        foreach (JArray account in accounts)
        {
            if (username == account[0].ToString() && password == account[1].ToString())
            {
                Debug.Log($"Login as {username}");
                loginPanel.SetActive(false);
                keyboard.SetActive(false);
                break;
            }
            else
                ErrorLoginPopUp.SetActive(true);
        }
    }
}
