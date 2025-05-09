using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForecastResultDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popUpResult;
    public Transform resultContainer;
    public GameObject buttonCloseForecast;
    public GameObject rowPrefab;
    public TextMeshProUGUI summaryText;

    private void Start()
    {
        popUpResult.SetActive(false);
        buttonCloseForecast.SetActive(false);
    }

    public void DisplayResults(float[] demandValues, float[] forecastData, float[] errors, float mad, float mse, float mape)
    {
        if (forecastData == null || forecastData.Length == 0)
        {
            Debug.LogError("Data forecasting tidak valid!");
            return;
        }

        ClearPreviousResults();

        popUpResult.SetActive(true);
        buttonCloseForecast.SetActive(true);

        // 🔥 Tambahkan Header Jika Belum Ada di UI
        GameObject headerRow = Instantiate(rowPrefab, resultContainer);
        TMP_Text[] headerTexts = headerRow.GetComponentsInChildren<TMP_Text>();
        headerTexts[0].text = "Period";
        headerTexts[1].text = "Demand";
        headerTexts[2].text = "Forecast";
        headerTexts[3].text = "Error";
        headerTexts[4].text = "|Error|";
        headerTexts[5].text = "Error²";
        headerTexts[6].text = "Pct Error";

        float totalDemand = 0f;
        float totalForecast = 0f;

        for (int i = 0; i < forecastData.Length; i++)
        {
            GameObject row = Instantiate(rowPrefab, resultContainer);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            texts[0].text = $"Period {i + 1}";
            texts[1].text = demandValues[i].ToString();
            texts[2].text = forecastData[i].ToString();
            texts[3].text = errors[i].ToString();
            texts[4].text = Mathf.Abs(errors[i]).ToString();
            texts[5].text = Mathf.Pow(errors[i], 2).ToString();
            texts[6].text = $"{Mathf.Abs(errors[i] / demandValues[i]) * 100:0.00}%";

            totalDemand += demandValues[i];
            totalForecast += forecastData[i];
        }

        summaryText.text = $"MAD: {mad:F2} | MSE: {mse:F2} | MAPE: {mape:F2}%";

        LayoutRebuilder.ForceRebuildLayoutImmediate(resultContainer.GetComponent<RectTransform>());

        Debug.Log("Forecasting results displayed successfully.");
    }

    public void CloseResultPanel()
    {
        popUpResult.SetActive(false);
        buttonCloseForecast.SetActive(false);
    }

    private void ClearPreviousResults()
    {
        foreach (Transform child in resultContainer)
        {
            Destroy(child.gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(resultContainer.GetComponent<RectTransform>());
    }
}