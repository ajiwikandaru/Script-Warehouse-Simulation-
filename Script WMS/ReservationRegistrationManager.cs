using Record;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

namespace CompanySystem
{
    public class ReservationRegistrationManager : MonoBehaviour
    {
        public TextMeshProUGUI reservationDate;
        public TMP_InputField clientInputField;

        private struct Date
        {
            public string day;
            public string month;
            public string year;
        }
        private Date selectedDate;
        public GameObject calendarPicker;

        public GameObject reservationTable;

        private void OnEnable()
        {
            selectedDate.day = DateTime.Now.Day.ToString();
            selectedDate.month = DateTime.Now.Month.ToString();
            selectedDate.year = DateTime.Now.Year.ToString();

            LoadDate();
        }

        public void AddNewReservation()
        {
            ReservationRecord newReservation = new ReservationRecord(reservationDate.text, clientInputField.text);
            var reservationData = new Dictionary<string, object>
            {
                { "code", newReservation.Code },
                { "reservation_date", newReservation.ReservationDate },
                { "client", newReservation.Client }
            };

            StartCoroutine(FirebaseServices.WriteData("reservations", reservationData, message =>
            {
                if (message.Contains("successfully"))
                {
                    ResetInput();
                    gameObject.SetActive(false);
                    RefreshTable();
                }
                else
                {
                    Debug.LogError(message);
                }
            }));
        }

        private void LoadDate()
        {
            reservationDate.text = $"{selectedDate.day}/{selectedDate.month}/{selectedDate.year}";
        }

        // Reset input field
        public void ResetInput()
        {
            if (clientInputField.text.Length > 0)
                clientInputField.text = clientInputField.text.Remove(0);
        }
        private void RefreshTable()
        {
            reservationTable.SetActive(false);
            reservationTable.SetActive(true);
        }
    }
}