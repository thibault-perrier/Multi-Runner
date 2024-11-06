using System;
using System.IO;

public class FileDataHandler
{
    private string _dataDirPath;
    private string _dataFileName;


    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
    }

    public GameData Load()
    {
        
        return null;
    }

    public void Save(GameData gameData)
    {
        string fullPath = Path.Combine(this._dataDirPath, this._dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e + "\n" +"Error occured while trying to save game data. at :" + fullPath + "\n");
            throw;
        }
    }
}
