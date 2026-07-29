using System;
using UnityEngine;

public class EventManager
{
    //смена главного персонажа (0 - оба, 1 - дед, 2 - внучка)
    public static Action<int> OnCharacterSelected;



    // Радиоволна 2: Подлетели/подошли к NPC, пора вызывать диалог (передаем текст)
    //public static Action<string> OnDialogueTriggered;

    //// Радиоволна 3: Диалог закрылся
    //public static Action OnDialogueClosed;
}
