using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarPickerManager : MonoBehaviour
{
    public TextMeshProUGUI selectedDate;

    private DateTime date;
    public TextMeshProUGUI month;
    public TextMeshProUGUI year;
    public Transform dayContainer;
    public GameObject dayTemplate;
    void OnEnable()
    {
        date = DateTime.ParseExact(selectedDate.text, "d/M/yyyy", CultureInfo.InvariantCulture);
        LoadCalendar();
    }

    void OnDisable()
    {
        DestroyDays();
    }

    private void LoadCalendar()
    {
        month.text = date.ToString("MMMM");
        year.text = date.ToString("yyyy");

        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        DestroyDays();
        for (int i = 0; i < daysInMonth; i++)
        {
            GameObject day = Instantiate(dayTemplate, dayContainer);
            day.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            day.GetComponent<Button>().onClick.AddListener(() => SelectDate(day.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text));
        }
        dayTemplate.SetActive(false);
    }

    public void DestroyDays()
    {
        foreach (Transform child in dayContainer)
        {
            if (child != dayTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }
        dayTemplate.SetActive(true);
    }

    public void PreviousMonth()
    {
        date = date.AddMonths(-1);
        LoadCalendar();
    }

    public void NextMonth()
    {
        date = date.AddMonths(1);
        LoadCalendar();
    }

    public void SelectDate(string day)
    {
        selectedDate.text = $"{day}/{date.Month}/{date.Year}";
    }
}