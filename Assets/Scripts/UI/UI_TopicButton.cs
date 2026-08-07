using UnityEngine;
using TMPro;

public class UI_TopicButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;

    // Метод, который вызывается из UI_DialogueWindow для установки текста кнопки
    public void Setup(string title)
    {
        if (buttonText != null)
        {
            buttonText.text = title;
        }
    }
}