using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicDropdown;
    public Text fullScreenLabel;
    public Toggle fullScreenToggle;

    private bool isInitialized = false;
    private bool isOpeningFromButton = false;

    void Start()
    {
        InitSettings();
        if (!isOpeningFromButton)
        {
            gameObject.SetActive(false);
        }
    }

    private void InitSettings()
    {
        if (isInitialized) {
            return;
        }

        int maxQualityIndex = QualitySettings.names.Length - 1;
        QualitySettings.SetQualityLevel(maxQualityIndex, true);

        if (graphicDropdown != null)
        {
            graphicDropdown.value = maxQualityIndex;
            graphicDropdown.RefreshShownValue();
        }

        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
        }

        UpdateFullScreenLabel(Screen.fullScreen);

        isInitialized = true;
    }

    public void OpenSettings()
    {
        isOpeningFromButton = true;
        gameObject.SetActive(true);
        InitSettings();
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    public void ChangedGraphicsQuality()
    {
        int index = graphicDropdown.value;
        QualitySettings.SetQualityLevel(index, true);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        UpdateFullScreenLabel(isFullscreen);
    }

    private void UpdateFullScreenLabel(bool isFullscreen)
    {
        if (fullScreenLabel != null)
        {
            fullScreenLabel.text = isFullscreen ? "on" : "off";
        }
    }
}