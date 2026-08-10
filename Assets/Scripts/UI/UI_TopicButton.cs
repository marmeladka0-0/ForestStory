using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TopicButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;

    private System.Action<int> onClickCallback;
    private int topicIndex;

    public void Setup(string title, int index, System.Action<int> onSelect)
    {
        topicIndex = index;
        onClickCallback = onSelect;

        if (buttonText != null)
        {
            buttonText.text = title;
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.interactable = true; // Reset interactability on creation
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    // Enables or disables the button
    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    private void OnButtonClicked()
    {
        onClickCallback?.Invoke(topicIndex);
    }
}