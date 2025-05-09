using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PomQmInputManager : ForecastItemManager
{
    public GameObject inputTemplate; // Template for displaying each record
    public Transform container; // Container to hold the instantiated records
    private InputFieldManager inputFieldManager;
    private List<TextMeshProUGUI> inputFields = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        inputFieldManager = FindObjectOfType<InputFieldManager>();
    }
    void OnEnable()
    {
        ShowInputField();
    }

    void ShowInputField()
    {
        float templateHigh = 91f;
        for (int i = 0; i < numberOfRecord; i++)
        {
            GameObject newRow = Instantiate(inputTemplate, container);
            Transform newRowTransform = newRow.transform;
            RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0f, -templateHigh * i);
            newRowTransform.Find("Past Period").GetComponent<TextMeshProUGUI>().text += (i + 1).ToString();
            //inputFields.Add(newRowTransform.Find("Text").GetComponent<TextMeshProUGUI>());
        }
        inputTemplate.SetActive(false);
    }

    public void SaveQuantityRecord()
    {
        // Bersihkan list yang ada untuk menghindari duplikasi
        quantity.Clear();

        for (int i = 0; i < numberOfRecord; i++)
        {
            string cleanedText = inputFields[i].text.Substring(0, inputFields[i].text.Length - 1);
            int buffer;
            int.TryParse(cleanedText, out buffer);
            quantity.Add(buffer);
        }
        Debug.Log("Saved quantities: " + string.Join(", ", quantity));
    }
}