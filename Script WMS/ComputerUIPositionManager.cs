using System;
using System.Collections;
using UnityEngine;
public class ComputerUIPositionManager : MonoBehaviour
{
    private Camera mainCamera; // User camera
    [SerializeField]
    private GameObject screen; // Computer screen game object
    [SerializeField]
    private GameObject keyboard; // Computer keyboard game object

    // Start is called before the first frame update
    void Start()
    {
        // Set main camera
        mainCamera = Camera.main;
    }

    // OnEnable is called when the object becomes enabled and active

    private void OnEnable()
    {
        StartCoroutine(SetScreenPosition());
    }

    // Set screen position
    private IEnumerator SetScreenPosition()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 screenPosition = screen.transform.position;
        screenPosition.y = cameraPosition.y;
        screen.transform.position = screenPosition;

        SetKeyboardPosition();
    }

    // Set keyboard position
    private void SetKeyboardPosition()
    {
        Vector3 screenPosition = screen.transform.position;
        Vector3 keyboardPosition = new Vector3(screenPosition.x - 0.1f, screenPosition.y - 0.1f, screenPosition.z);
        keyboard.transform.position = keyboardPosition;
    }
}