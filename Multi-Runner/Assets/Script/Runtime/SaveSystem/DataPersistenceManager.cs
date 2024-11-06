using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    
    private GameData _gameData;
    
    private List<IDataPersistence> _dataPersistenceObjects;
    
    public static DataPersistenceManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one DataPersistenceManager in scene.");
        }
        Instance = this;
    }

    void Start()
    {
        this._dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataPersistenceObjects);
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

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(_gameData);
        }
    }


    public void SaveGame()
    {
        // TODO - Get the game data from the script
        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref _gameData);
        }

        //TODO - Save the game data in a file
    }
    
    
    [ContextMenu("Save Game")]
    public void Save() { SaveGame(); }
    
    [ContextMenu("Load Game")]
    public void Load() { LoadGame(); }
    
    [ContextMenu("New Game")]
    public void New() { NewGame(); }
    
}
