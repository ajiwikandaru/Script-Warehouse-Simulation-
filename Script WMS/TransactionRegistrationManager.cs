
using Record;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class TransactionRegistrationManager : MonoBehaviour
    {
        public TMP_InputField invoiceNumberInputField;
        public TextMeshProUGUI invoiceDate;
        public TMP_InputField vendorInputField;

        private struct Date
        {
            public string day;
            public string month;
            public string year;
        }
        private Date selectedDate;
        public GameObject calendarPicker;

        public GameObject transactionTable;

        public GameObject popup;
        private GameObject warningPanel;

        private void OnEnable()
        {
            selectedDate.day = DateTime.Now.Day.ToString();
            selectedDate.month = DateTime.Now.Month.ToString();
            selectedDate.year = DateTime.Now.Year.ToString();

            LoadDate();
        }

        public void AddNewTransaction()
        {
            TransactionRecord newTransaction = new TransactionRecord(invoiceNumberInputField.text, invoiceDate.text, vendorInputField.text);
            var transactionData = new Dictionary<string, object>
            {
                { "code", newTransaction.Code },
                { "invoice_number", newTransaction.InvoiceNumber },
                { "invoice_date", newTransaction.InvoiceDate },
                { "vendor", newTransaction.Vendor }
            };

            StartCoroutine(FirebaseServices.WriteData("transactions", transactionData, "invoice_number", message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                }
                else if (message.Contains("registered"))
                {
                    ShowPopup();
                    message = $"Transaction with invoice number {transactionData["invoice_number"]} has been registered";
                    TransactionRegisteredHandler(message);
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }

        private void LoadDate()
        {
            invoiceDate.text = $"{selectedDate.day}/{selectedDate.month}/{selectedDate.year}";
        }

        // Reset input field
        public void ResetInput()
        {
            if (invoiceNumberInputField.text.Length > 0)
                invoiceNumberInputField.text = invoiceNumberInputField.text.Remove(0);
            if (vendorInputField.text.Length > 0)
                vendorInputField.text = vendorInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            transactionTable.SetActive(false);
            transactionTable.SetActive(true);
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

        private void TransactionRegisteredHandler(string message)
        {
            warningPanel = popup.transform.Find("Transaction Registered").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message;
        }
    }
}
