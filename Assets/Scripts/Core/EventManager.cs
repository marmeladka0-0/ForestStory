using System;
using UnityEngine;

public class EventManager
{
    //main character is changed => update a status/event here
    public static Action<int> OnCharacterSelected;

    //normaly all logic should be here
    //and on other scripts all variables should be private
    //we need some fix of logic even for now((

    //if dialog with npc is triggered
    //public static Action<string> OnDialogueTriggered;

    //If we stop a dialog
    //public static Action OnDialogueClosed;


    //maybe even step sound logic should be here
    //anyway need fixes
}
