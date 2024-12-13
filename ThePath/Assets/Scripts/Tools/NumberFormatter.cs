using UnityEngine;
using TMPro;

public class NumberFormatter : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    [SerializeField] private string _ThousandShort = "K";
    [SerializeField] private string _MillionShort = "M";
    [SerializeField] private string _BillionShort = "B";

    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        if (textMeshPro != null)
        {
            textMeshPro.text = FormatTextWithIcon(textMeshPro.text);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned or found.");
        }
    }

    private string FormatTextWithIcon(string text)
    {
        // Find the index where the number starts
        int numberStartIndex = text.LastIndexOf('>') + 1;

        // Extract the number part from the text
        string numberString = text.Substring(numberStartIndex);

        if (float.TryParse(numberString, out float num))
        {
            string formattedNumber = FormatNumber(num);
            return text.Substring(0, numberStartIndex) + formattedNumber;
        }
        else
        {
            Debug.LogError("The text does not contain a valid number.");
            return text;
        }
    }

    private string FormatNumber(float num)
    {
        if (num >= 1000000000)
        {
            return (num / 1000000000f).ToString("0.##") + _BillionShort;
        }
        else if (num >= 1000000)
        {
            return (num / 1000000f).ToString("0.##") + _MillionShort;
        }
        else if (num >= 1000)
        {
            return (num / 1000f).ToString("0.##") + _ThousandShort;
        }
        else
        {
            return num.ToString("0");
        }
    }
}