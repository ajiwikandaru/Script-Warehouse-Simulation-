using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForecastSelectionHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown dropdownMethod;
    public TMP_InputField inputAlpha;
    public DemandInputGenerator demandInputGenerator;
    public ForecastResultDisplay forecastResultDisplay;
    public GameObject panelInputData;
    public GameObject buttonGenerateInputs;
    public GameObject buttonStartForecasting;
    public GameObject buttonSelectMethod;

    private bool methodSelected = false;
    private bool inputFieldsGenerated = false;

    private void Start()
    {
        if (demandInputGenerator == null || forecastResultDisplay == null || dropdownMethod == null)
        {
            Debug.LogError("Komponen penting belum dihubungkan di Inspector!");
            return;
        }

        InitializeUI();
        dropdownMethod.onValueChanged.AddListener(OnMethodChanged);

        buttonGenerateInputs.GetComponent<Button>().onClick.AddListener(GenerateDemandInputs);
        buttonSelectMethod.GetComponent<Button>().onClick.AddListener(ShowForecastMethods);
    }

    private void InitializeUI()
    {
        panelInputData.SetActive(false);
        buttonGenerateInputs.SetActive(false);
        inputAlpha.gameObject.SetActive(false);
        dropdownMethod.gameObject.SetActive(false);
        buttonStartForecasting.SetActive(false);
    }

    public void ShowForecastMethods()
    {
        if (!methodSelected)
        {
            dropdownMethod.gameObject.SetActive(true);
            buttonSelectMethod.SetActive(false);
            methodSelected = true;
        }
    }

    private void OnMethodChanged(int index)
    {
        panelInputData.SetActive(true);
        buttonGenerateInputs.SetActive(true);
        buttonStartForecasting.SetActive(true);

        string selectedMethod = dropdownMethod.options[index].text;
        inputAlpha.gameObject.SetActive(selectedMethod == "Exponential Smoothing");

        demandInputGenerator.ClearInputFields();
        inputFieldsGenerated = false;
    }

    public void GenerateDemandInputs()
    {
        Debug.Log("Generating demand inputs...");
        panelInputData.SetActive(true);
        demandInputGenerator.GenerateInputFields();
        inputFieldsGenerated = true;
    }

    public void ProceedToForecasting()
    {
        if (!inputFieldsGenerated)
        {
            Debug.LogError("InputField Demand belum dibuat! Klik 'Generate Inputs' terlebih dahulu.");
            return;
        }

        float[] demandValues = demandInputGenerator.GetDemandValues();
        if (demandValues == null || demandValues.Length == 0)
        {
            Debug.LogError("Pastikan semua InputField Demand telah diisi dengan angka yang valid!");
            return;
        }

        string selectedMethod = dropdownMethod.options[dropdownMethod.value].text;
        Debug.Log($"Metode Forecasting yang dipilih: {selectedMethod}");

        float[] forecastResults, errors;
        float mad, mse, mape;

        if (selectedMethod == "Moving Average")
        {
            int period = 3; // Bisa diganti sesuai kebutuhan
            (forecastResults, errors, mad, mse, mape) = ForecastCalculator.CalculateMovingAverage(demandValues, period);
        }
        else if (selectedMethod == "Exponential Smoothing")
        {
            float alpha = float.Parse(inputAlpha.text);
            (forecastResults, errors, mad, mse, mape) = ForecastCalculator.CalculateExponentialSmoothing(demandValues, alpha);
        }
        else
        {
            Debug.LogError("Metode forecasting tidak valid!");
            return;
        }

        forecastResultDisplay.DisplayResults(demandValues, forecastResults, errors, mad, mse, mape);
    }
}