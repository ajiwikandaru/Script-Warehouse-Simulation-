using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class DeleteItemManager : MonoBehaviour
    {
        string sku;
        public TextMeshProUGUI warningText;

        public GameObject itemTable;
        private void OnEnable()
        {
            sku = ItemListManager.selectedRecord.Sku;
            warningText.text = $"Data item with sku {sku} will be deleted.\r\nAre you sure?";
        }
        public void DeleteItem()
        {
            StartCoroutine(FirebaseServices.DeleteData("items", "sku", sku, message =>
            {
                if (message.Contains("successfully deleted"))
                {
                    transform.parent.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    RefreshTable();
                    ItemListManager.ResetSelectedRecord();
                }
                else if (message.Contains("No data found"))
                {
                    Debug.LogWarning("Delete failed: No data found with the specified primary key.");
                }
                else
                {
                    Debug.LogError("Delete failed: " + message);
                }
            }));
        }

        private void RefreshTable()
        {
            itemTable.SetActive(false);
            itemTable.SetActive(true);
        }
    }
}