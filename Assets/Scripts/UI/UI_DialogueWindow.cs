using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_DialogueWindow : MonoBehaviour
{
    [Header("UI Ссылки")]
    [SerializeField] private Image speakerAvatar;
    [SerializeField] private TMP_Text speakerNameText; // Если есть поле имени
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform topicButtonsContainer;

    [Header("Префабы")]
    [SerializeField] private GameObject topicButtonPrefab;

    private void Awake()
    {
        CloseDialogue();
    }

    // Принимаем данные напрямую из NPCInteractable
    public void OpenDialogue(Sprite avatar, string characterName, string speechText, string[] topics)
    {
        gameObject.SetActive(true);

        if (speakerAvatar != null) speakerAvatar.sprite = avatar;
        if (speakerNameText != null) speakerNameText.text = characterName;
        if (dialogueText != null) dialogueText.text = speechText;

        // Очищаем старые кнопки
        foreach (Transform child in topicButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые кнопки под темы
        if (topics != null)
        {
            foreach (string topicTitle in topics)
            {
                GameObject newButton = Instantiate(topicButtonPrefab, topicButtonsContainer);
                if (newButton.TryGetComponent<UI_TopicButton>(out var topicButtonScript))
                {
                    topicButtonScript.Setup(topicTitle);
                }
            }
        }
    }

    public void CloseDialogue()
    {
        gameObject.SetActive(false);
    }
}