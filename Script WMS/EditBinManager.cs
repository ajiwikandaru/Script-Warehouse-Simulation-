using Record;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CompanySystem
{
    public class EditBinManager : MonoBehaviour
    {
        public TMP_InputField codeInputField;
        public TMP_InputField informationInputField;

        public GameObject binTable;

        public GameObject popup;
        private GameObject warningPanel;

        void OnEnable()
        {
            codeInputField.text = BinListManager.selectedRecord.Code.ToString();
            informationInputField.text = BinListManager.selectedRecord.Information.ToString();
        }
        public void EditBin()
        {
            BinRecord newBin = new BinRecord(codeInputField.text, informationInputField.text);
            string oldCode = BinListManager.selectedRecord.Code;

            var newBinData = new Dictionary<string, object>
            {
                { "code", newBin.Code },
                { "information", newBin.Information },
                { "number_of_tags", newBin.NumberOfTags },
                { "active", newBin.Active }
            };

            StartCoroutine(FirebaseServices.ModifyData("bins", newBinData, oldCode, "code", message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                    BinListManager.ResetSelectedRecord();
                }
                else if (message.Contains("registered"))
                {
                    ShowPopup();
                    BinRegisteredHandler(message, newBin.Code);
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }

        // Reset input field
        public void ResetInput()
        {
            if (codeInputField.text.Length > 0)
                codeInputField.text = codeInputField.text.Remove(0);
            if (informationInputField.text.Length > 0)
                informationInputField.text = informationInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            binTable.SetActive(false);
            binTable.SetActive(true);
        }

        private void ShowPopup()
        {
            popup.SetActive(true);
        }

        private void HidePopup()
        {
            if (warningPanel != null && warningPanel.activeSelf)
            {
                warningPanel.transform.parent.gameObject.SetActive(false);
                warningPanel.SetActive(false);
            }
        }

        private void BinRegisteredHandler(string message, string newCode)
        {
            warningPanel = popup.transform.Find("Bin Registered").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message;

            GameObject replaceBinButton = warningPanel.transform.Find("Buttons").Find("Yes Button").gameObject;

            // Add event trigger to replace bin button
            EventTrigger trigger = replaceBinButton.GetComponent<EventTrigger>() ?? replaceBinButton.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            // Create entry for click/ pointer down event
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            entry.callback.AddListener((eventData) =>
            {
                StartCoroutine(FirebaseServices.DeleteData("bins", "code", newCode, deleteResult =>
                {
                    if (deleteResult.Contains("successfully"))
                    {
                        EditBin();
                    }
                }));
            });
            trigger.triggers.Add(entry);
        }
    }
}