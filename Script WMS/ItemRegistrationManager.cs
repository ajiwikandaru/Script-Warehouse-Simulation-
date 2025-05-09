using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Record;
using UnityEngine.EventSystems;

namespace CompanySystem
{
    public class ItemRegistrationManager : MonoBehaviour
    {
        public TMP_InputField skuInputField;
        public TMP_InputField itemNameInputField;
        public TMP_Dropdown binCodeDropdown;
        public TMP_Dropdown uomDropdown;

        public GameObject itemTable;

        public GameObject popup;
        private GameObject warningPanel;

        private void OnEnable()
        {
            // Invoke GetData method after a short delay
            Invoke("GetBinCodes", 0.1f);
        }

        private void OnDisable()
        {
            binCodeDropdown.options.Clear();
        }

        public void GetBinCodes()
        {
            StartCoroutine(FirebaseServices.ReadData("bins", data =>
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        binCodeDropdown.options.Add(new TMP_Dropdown.OptionData(item["code"].ToString()));
                    }
                    binCodeDropdown.captionText.text = binCodeDropdown.options[0].text;
                }
                else
                {
                    Debug.LogError("Failed to retrieve data.");
                }
            }));
        }

        // Register new item to system
        public void AddNewItem()
        {
            ItemRecord newItem = new ItemRecord(skuInputField.text, itemNameInputField.text, binCodeDropdown.captionText.text, uomDropdown.captionText.text);
            var itemData = new Dictionary<string, object>
            {
                { "sku", newItem.Sku },
                { "item_name", newItem.ItemName },
                { "bin_code", newItem.BinCode },
                { "quantity", newItem.Quantity },
                { "uom", newItem.UOM },
                { "active", newItem.Active },
                { "number_of_tags", newItem.NumberOfTags }
            };

            StartCoroutine(FirebaseServices.WriteData("items", itemData, "bin_code", "sku", message =>
            {
                if (message.Contains("successfully"))
                {
                    HidePopup();
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                }
                else if (message.Contains("registered") && message.Contains("Replace"))
                {
                    ShowPopup();
                    ItemRegisteredHandler(message, newItem.Sku);
                }
                else if (message.Contains("registered") && !message.Contains("Replace"))
                {
                    ShowPopup();
                    message = $"Bin with bin code {newItem.BinCode} already in use";
                    BinInUseHandler(message);
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
            if (skuInputField.text.Length > 0)
                skuInputField.text = skuInputField.text.Remove(0);
            if (itemNameInputField.text.Length > 0)
                itemNameInputField.text = itemNameInputField.text.Remove(0);

            binCodeDropdown.options.Clear();
        }
        private void RefreshTable()
        {
            itemTable.SetActive(false);
            itemTable.SetActive(true);
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

        private void ItemRegisteredHandler(string message, string sku)
        {
            warningPanel = popup.transform.Find("Item Registered").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message;

            GameObject replaceItemButton = warningPanel.transform.Find("Buttons").Find("Yes Button").gameObject;

            // Add event trigger to replace bin button
            EventTrigger trigger = replaceItemButton.GetComponent<EventTrigger>() ?? replaceItemButton.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            // Create entry for click/ pointer down event
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            entry.callback.AddListener((eventData) =>
            {
                StartCoroutine(FirebaseServices.DeleteData("items", "sku", sku, deleteResult =>
                {
                    if (deleteResult.Contains("successfully"))
                    {
                        AddNewItem();
                    }
                }));
            });
            trigger.triggers.Add(entry);
        }

        private void BinInUseHandler(string message)
        {
            warningPanel = popup.transform.Find("Bin In Use").gameObject;
            warningPanel.SetActive(true);

            TextMeshProUGUI warningText = warningPanel.GetComponentInChildren<TextMeshProUGUI>();
            warningText.text = message;
        }
    }
}