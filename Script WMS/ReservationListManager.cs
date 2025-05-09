
using Newtonsoft.Json.Linq;
using Record;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompanySystem
{
    public class ReservationListManager : MonoBehaviour
    {
        private JArray reservations; // Array to store reservations data
        public GameObject Table;
        public Transform container; // Container to hold the instantiated records
        public GameObject recordTemplate; // Template for displaying each record
        public static ReservationRecord selectedRecord; // Variabel to hold selected record data

        public GameObject detailPage; // Page to open detail selected record data

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

        // Get all reservation data from Database
        public void GetData()
        {
            StartCoroutine(FirebaseServices.ReadData("reservations", data =>
            {
                if (data != null)
                {
                    reservations = data;
                    ShowRecord();
                }
                else
                {
                    recordTemplate.SetActive(false);
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        // Display all reservation data to reservation table
        void ShowRecord()
        {
            float templateHigh = 91f;
            for (int i = 0; i < reservations.Count; i++)
            {
                GameObject newRow = Instantiate(recordTemplate, container);
                Transform newRowTransform = newRow.transform;
                RectTransform entryRectTransform = newRow.GetComponent<RectTransform>();

                // Fill the UI elements with data
                entryRectTransform.anchoredPosition = new Vector2(0f, 228f + (-templateHigh * i));
                newRowTransform.Find("Code").GetComponent<TextMeshProUGUI>().text = reservations[i]["code"].ToString();
                newRowTransform.Find("Reservation Date").GetComponent<TextMeshProUGUI>().text = reservations[i]["reservation_date"].ToString();
                newRowTransform.Find("Client").GetComponent<TextMeshProUGUI>().text = reservations[i]["client"].ToString();

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

            StartCoroutine(FirebaseServices.ReadData("reservations", "code", code, data =>
            {
                if (data != null)
                {
                    ReservationRecord record = new ReservationRecord(data);
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

        private IEnumerator OpenDetailPage(Transform recordTransform)
        {
            GetRecord(recordTransform);

            yield return new WaitUntil(() => selectedRecord.Code != null);
            gameObject.SetActive(false);
            detailPage.SetActive(true);
        }

        public static void ResetSelectedRecord()
        {
            ReservationRecord emptyReservation = new ReservationRecord();
            selectedRecord = emptyReservation;
        }

        // Delete shows all reservation data in the reservation table
        public void DestroyRecord()
        {
            foreach (Transform child in container)
            {
                if (child != recordTemplate.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            recordTemplate.SetActive(true);
        }
    }
}
