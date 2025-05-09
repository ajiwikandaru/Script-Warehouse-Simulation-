using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PomQmForecastManager : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI numberOfPeriods;
    public List<GameObject> inputPages = new List<GameObject>();
    protected static string datasetName;
    protected static int numberOfRecords;
    protected static string method;
    protected static List<int> quantity = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        numberOfPeriods.text = 0.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected static string CleaningText(TextMeshProUGUI rawText)
    {
        string cleanedText = rawText.text.Substring(0, rawText.text.Length - 1);
        return cleanedText;
    }

    private int StringToInt(string text)
    {
        int integerNumber;
        int.TryParse(text, out integerNumber);
        return integerNumber;
    }

    public void PlusOnePeriods()
    {
        int buffer;
        int.TryParse(numberOfPeriods.text, out buffer);
        numberOfPeriods.text = (buffer + 1).ToString();
    }

    public void MinusOnePeriods()
    {
        int buffer;
        int.TryParse(numberOfPeriods.text, out buffer);
        numberOfPeriods.text = (buffer - 1).ToString();
    }

    public void CreateDataset()
    {
        datasetName = title.text;
        numberOfRecords = StringToInt(numberOfPeriods.text);
    }

    public void OpenMovingAveragePage()
    {

    }
}
