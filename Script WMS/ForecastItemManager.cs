using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForecastItemManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI numberOfPeriods;
    protected static int numberOfRecord;
    protected static List<int> quantity = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        numberOfRecord = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InputNumberOfRecord()
    {
        string rawText = numberOfPeriods.text;
        string cleanedText = rawText.Substring(0, rawText.Length - 1);
        int.TryParse(cleanedText, out numberOfRecord);
    }
}