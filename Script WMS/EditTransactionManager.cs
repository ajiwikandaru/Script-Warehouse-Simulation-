
using Record;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class EditTransactionManager : MonoBehaviour
    {
        public TMP_InputField invoiceNumberInputField;
        public TextMeshProUGUI invoiceDate;
        public TMP_InputField vendorInputField;

        public GameObject calendarPicker;

        public GameObject transactionTable;

        public GameObject popup;
        private GameObject warningPanel;

        void OnEnable()
        {
            invoiceNumberInputField.text = TransactionListManager.selectedRecord.InvoiceNumber;
            vendorInputField.text = TransactionListManager.selectedRecord.Vendor;

            LoadDate();
        }
        public void EditTransaction()
        {
            TransactionRecord newTransaction = new TransactionRecord(invoiceNumberInputField.text, invoiceDate.text, vendorInputField.text);
            string oldInvoiceNumber = TransactionListManager.selectedRecord.InvoiceNumber;

            var newTransactionData = new Dictionary<string, object>
            {
                { "invoice_number", newTransaction.InvoiceNumber },
                { "invoice_date", newTransaction.InvoiceDate },
                { "vendor", newTransaction.Vendor }
            };

            StartCoroutine(FirebaseServices.ModifyData("transactions", newTransactionData, oldInvoiceNumber, "invoice_number", message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                    TransactionListManager.ResetSelectedRecord();
                }
                else if (message.Contains("registered"))
                {
                    ShowPopup();
                    message = $"Transaction with invoice number {newTransactionData["invoice_number"]} has been registered";
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
            invoiceDate.text = TransactionListManager.selectedRecord.InvoiceDate;
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
