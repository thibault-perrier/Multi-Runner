using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    
    private GameData _gameData;
    
    public static DataPersistenceManager Instance { get; private set; }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one DataPersistenceManager in scene.");
        }
        Instance = this;
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO - Load game data from file
        if (_gameData == null)
        {
            Debug.Log("No game data was found. Initializing new game data");
            _gameData = new GameData();
        }
        // TODO - set game data loaded in the script 
    }


    public void SaveGame()
    {
        // TODO - Get the game data from the script
        
        //TODO - Save the game data in a file
    }
    
    
    [ContextMenu("Save Game")]
    public void Save() { SaveGame(); }
    
    [ContextMenu("Load Game")]
    public void Load() { LoadGame(); }
    
    [ContextMenu("New Game")]
    public void New() { NewGame(); }
    
}
