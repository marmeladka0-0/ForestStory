using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Character References")]
    [SerializeField] private CharacterController2D grandfather;
    [SerializeField] private CharacterController2D granddaughter;

    // Cross-platform save file path
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Hotkeys for testing
        // F5 - Save Game
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        // F9 - Load Game
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 1. Save character positions
        if (grandfather != null) data.grandfatherPosition = grandfather.transform.position;
        if (granddaughter != null) data.granddaughterPosition = granddaughter.transform.position;

        // 2. Save unlocked journal entries (when JournalManager is added)
        /*
        foreach (var note in JournalManager.Instance.GetUnlockedNotes())
        {
            data.unlockedNoteIDs.Add(note.noteID);
        }
        */

        // 3. Serialize to JSON format and write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"<color=green>Game saved to:</color> {saveFilePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found!");
            return;
        }

        // 1. Read JSON file content
        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 2. Restore character positions and reset movement targets
        if (grandfather != null)
        {
            grandfather.TeleportTo(data.grandfatherPosition);
        }

        if (granddaughter != null)
        {
            granddaughter.TeleportTo(data.granddaughterPosition);
        }

        // 3. Restore journal entries by ID
        /*
        if (JournalManager.Instance != null)
        {
            JournalManager.Instance.RestoreNotesFromIDs(data.unlockedNoteIDs);
        }
        */

        Debug.Log("<color=cyan>Game successfully loaded!</color>");
    }

    // Delete save file (resets game progress)
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}