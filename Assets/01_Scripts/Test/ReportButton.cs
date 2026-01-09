using UnityEngine;

public class ReportButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Report()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfaYyqL5W4ALV9wWXCTKD6EGMyfHO0jdB4oxkT8dEyNvlFYPw/viewform");
    }
}
