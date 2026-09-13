using UnityEngine;

public class QuitSceneManager : MonoBehaviour
{
    public UI_Manager uiManager;

    public void OnReturnHomeButton()
    {
        if (uiManager != null)
        {
            uiManager.HomePage();
        }
        else
        {
            Application.OpenURL("https://itch.io/");
        }
    }
}
