using UnityEngine;
using System;
using UnityEngine.UI;

public class WeekInfo : MonoBehaviour
{
    void Start()
    {
        DateTime today = DateTime.Today;

        int weekNumber = GetIsoWeekOfYear(today);
        bool isEvenWeek = weekNumber % 2 == 0;
        string dayOfWeek = today.DayOfWeek.ToString();
        string dateString = today.ToString("dd.MM.yyyy");
        string result = $"{(isEvenWeek ? "Чётная" : "Нечётная")} неделя, {GetRussianDayName(dayOfWeek)}, {dateString}";

        gameObject.GetComponent<Text>().text = result;

    }


    private int GetIsoWeekOfYear(DateTime date)
    {
        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        return cal.GetWeekOfYear(date,
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
    }

    private string GetRussianDayName(string englishDayName)
    {
        switch (englishDayName)
        {
            case "Monday": return "Понедельник";
            case "Tuesday": return "Вторник";
            case "Wednesday": return "Среда";
            case "Thursday": return "Четверг";
            case "Friday": return "Пятница";
            case "Saturday": return "Суббота";
            case "Sunday": return "Воскресенье";
            default: return "cooked 💀";
        }
    }
}