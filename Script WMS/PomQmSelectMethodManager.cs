using UnityEngine;
using TMPro;

public class PomQmSelectMethodManager : PomQmForecastManager
{
    public TextMeshProUGUI selectedMethod;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveMethod()
    {
        method = selectedMethod.text;
    }
}
