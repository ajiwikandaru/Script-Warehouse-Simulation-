using UnityEngine;
using TMPro;

public class DemandInputGenerator : MonoBehaviour
{
    [Header("Prefab & Container")]
    public GameObject inputFieldPrefab; // Prefab untuk InputField Demand
    public Transform inputFieldContainer; // Tempat menyusun InputField secara dinamis

    [Header("User Input")]
    public TMP_InputField amountInputField; // InputField jumlah demand yang ingin dibuat

    private const int MAX_INPUTS = 24; // Batas maksimum InputField Demand
    private TMP_InputField[] inputFields; // Array untuk menyimpan referensi InputField yang dibuat

    private float initialX = 1000f; // Geser posisi ke kiri agar label tidak bertabrakan
    private float initialY = 220f;  // Naikkan posisi awal agar tidak terlalu ke bawah
    private float offsetY = -55f;   // Jarak antar InputField Demand

    public void GenerateInputFields()
    {
        if (!ValidateAmountInput(out int requestedAmount))
        {
            Debug.LogError("Masukkan angka valid untuk jumlah InputField!");
            return;
        }

        int numberOfInputs = Mathf.Clamp(requestedAmount, 1, MAX_INPUTS); // Batasi ke 1-24

        ClearInputFields(); // Hapus InputField lama sebelum membuat yang baru

        inputFields = new TMP_InputField[numberOfInputs];

        for (int i = 0; i < numberOfInputs; i++)
        {
            GameObject inputFieldObj = Instantiate(inputFieldPrefab, inputFieldContainer);
            TMP_InputField inputField = inputFieldObj.GetComponent<TMP_InputField>();
            inputField.placeholder.GetComponent<TMP_Text>().text = $"Demand {i + 1}";
            inputFields[i] = inputField;

            // Set posisi InputField agar tersusun ke kiri dan lebih tinggi
            RectTransform rectTransform = inputFieldObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(initialX, initialY + (i * offsetY));

            // Update label "Past Period X" di sebelah kiri InputField Demand
            Transform labelTransform = inputFieldObj.transform.Find("Label_Text");
            if (labelTransform != null)
            {
                TMP_Text label = labelTransform.GetComponent<TMP_Text>();
                label.text = $"Past Period {i + 1}";
                label.color = Color.black; // Warna teks hitam
                label.alignment = TextAlignmentOptions.Left; // Pastikan teks sejajar dalam satu baris

                // Beri extra spacing agar angka 1 tidak turun ke bawah
                label.text = $"Past Period {i + 1}";

                RectTransform labelRect = labelTransform.GetComponent<RectTransform>();
                labelRect.anchoredPosition = new Vector2(-250f, 0f); // Geser lebih ke kiri agar label lebih lapang
                labelRect.sizeDelta = new Vector2(250f, labelRect.sizeDelta.y); // Lebarkan label agar muat teks dan angka
            }
        }

        Debug.Log($"Generated {numberOfInputs} InputFields with labels.");
    }

    private bool ValidateAmountInput(out int amount)
    {
        return int.TryParse(amountInputField.text, out amount);
    }

    public void ClearInputFields()
    {
        foreach (Transform child in inputFieldContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public float[] GetDemandValues()
    {
        if (inputFields == null || inputFields.Length == 0)
        {
            Debug.LogError("Tidak ada InputField untuk mengambil nilai Demand!");
            return null;
        }

        float[] demands = new float[inputFields.Length];

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(inputFields[i].text))
            {
                Debug.LogError($"Input tidak valid di baris {i + 1}: Kosong!");
                return null;
            }

            if (float.TryParse(inputFields[i].text, out float value))
            {
                demands[i] = value;
            }
            else
            {
                Debug.LogError($"Input tidak valid di baris {i + 1}: Bukan angka!");
                return null;
            }
        }

        return demands;
    }
}