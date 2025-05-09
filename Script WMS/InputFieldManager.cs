using Keyboard;
using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{
    public KeyboardManager keyboardManager; // Reference to the keyboard manager

    // Set the output field of the keyboard manager to the input field
    public void KeyboardInput(TMP_InputField inputField)
    {
        keyboardManager.outputField = inputField;
    }
}