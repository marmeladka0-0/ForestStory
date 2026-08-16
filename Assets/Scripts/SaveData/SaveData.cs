using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<string> unlockedNoteIDs = new List<string>();

    public Vector3 grandfatherPosition;
    public Vector3 granddaughterPosition;

    public SaveData()
    {
        unlockedNoteIDs = new List<string>();
        grandfatherPosition = Vector3.zero;
        granddaughterPosition = Vector3.zero;
    }
}
