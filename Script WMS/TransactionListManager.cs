//transactionlistmanager

using Newtonsoft.Json.Linq;
using Record;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompanySystem
{
    public class TransactionListManager : MonoBehaviour
    {
        private JArray transactions; // Array to store transactions data
        public GameObject Table;
        public Transform container; // Container to hold the instantiated records
        public GameObject recordTemplate; // Template for displaying each record
        public static TransactionRecord selectedRecord; // Variabel to hold selected record data

        public GameObject detailPage; // Page to open detail selected record data
        public GameObject editButton; // Button to edit selected record data

        // Start is called before the first frame update
        void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("GetData", 0.1f);

            ResetSelectedRecord();
        }

        private void OnDisable()
        {
            DestroyRecord();
        }

        // Get all transaction data from Database
        public void GetData()
        {
            StartCoroutine(FirebaseServices.ReadData("transactions", data =>
            {
                if (data != null)
                {
                    transactions = data;
                    ShowRecord();
                }
                else
                {
                    recordTemplate.SetActive(false);
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        // Display all transaction data to transaction table
        void ShowRecord()
        {
            float templateHigh = 91f;
            for (int i = 0; i < transactions.Count; i++)
            {
                GameObject newRow = Instantiate(recordTemplate, container);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * i));
                newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = transactions[i]["code"].ToString();
                newRowTransform.Find("Invoice Number").GetComponent<TextMeshProUGUI>().text = transactions[i]["invoice_number"].ToString();
                newRowTransform.Find("Invoice Date").GetComponent<TextMeshProUGUI>().text = transactions[i]["invoice_date"].ToString();
                newRowTransform.Find("Vendor").GetComponent<TextMeshProUGUI>().text = transactions[i]["vendor"].ToString();

                if (i % 2 != 0)
                {
                    newRowTransform.Find("Record Background").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }

                Table.GetComponent<DynamicTableManager>().enabled = true;
            }
            recordTemplate.SetActive(false);
        }

        private void GetRecord(Transform recordTransform)
        {
            string code = recordTransform.Find("Code").GetComponent<TextMeshProUGUI>().text;

            StartCoroutine(FirebaseServices.ReadData("transactions", "code", code, data =>
            {
                if (data != null)
                {
                    TransactionRecord record = new TransactionRecord(data);
                    selectedRecord = record;
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        public void OnSettingButtonClick(Transform recordTransform)
        {
            StartCoroutine(OpenDetailPage(recordTransform));
        }

        public void OnExpandButtonClick(Transform recordTransform)
        {
            StartCoroutine(ShowEditButton(recordTransform));
        }

        private IEnumerator OpenDetailPage(Transform recordTransform)
        {
            GetRecord(recordTransform);

            yield return new WaitUntil(() => selectedRecord.Code != null);
            gameObject.SetActive(false);
            detailPage.SetActive(true);
        }

        public IEnumerator ShowEditButton(Transform recordTransform)
        {
            GetRecord(recordTransform);

            yield return new WaitForSeconds(0.1f);
            // Show edit button under expand button
            editButton.SetActive(true);
            float recordY = recordTransform.GetComponent<RectTransform>().anchoredPosition.y;
            RectTransform editButtonRectTransform = editButton.GetComponent<RectTransform>();
            editButtonRectTransform.anchoredPosition = new Vector2(editButtonRectTransform.anchoredPosition.x, recordY - 46f);
            editButton.transform.SetAsLastSibling();
        }

        public static void ResetSelectedRecord()
        {
            TransactionRecord emptyTransaction = new TransactionRecord();
            selectedRecord = emptyTransaction;
        }

        // Delete shows all transaction data in the transaction table
        public void DestroyRecord()
        {
            foreach (Transform child in container)
            {
                if (child != recordTemplate.transform && child != editButton.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            recordTemplate.SetActive(true);
            editButton.SetActive(false);
        }
    }
}