using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class DeleteBinManager : MonoBehaviour
    {
        string code;
        public TextMeshProUGUI warningText;

        public GameObject binTable;
        private void OnEnable()
        {
            code = BinListManager.selectedRecord.Code;
            warningText.text = $"Data bin with code {code} will be deleted.\r\nAre you sure?";
        }
        public void DeleteBin()
        {
            StartCoroutine(FirebaseServices.DeleteData("bins", "code", code, message =>
            {
                if (message.Contains("successfully deleted"))
                {
                    transform.parent.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    RefreshTable();
                    BinListManager.ResetSelectedRecord();
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
            binTable.SetActive(false);
            binTable.SetActive(true);
        }
    }
}