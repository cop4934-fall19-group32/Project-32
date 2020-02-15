using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    /* Set in Unity to select save file format */
    public bool UseJsonSaves;

    /* Used to ensure singletonness */
    private static PlayerState instance;

    /** The currently loaded save file */
    private Save ActiveSave;

    /** The save slot associated with the save file */
    private int SaveSlot;

    /* Directory in which save files will be placed */
    private string SaveDirectory;

    /* Variables to control binary save file properties */
    private string BinarySaveFileName;
    private string BinarySaveFileExtension;

    /* Variables to control JSON save file properties */
    private string JsonSaveFileName;
    private string JsonSaveFileExtension;

    private void Awake() 
    {
        SaveDirectory = Application.persistentDataPath + "/";
        BinarySaveFileName = "ComputronSave";
        BinarySaveFileExtension = ".save";
        
        JsonSaveFileName = "ComputronSaveDebug";
        JsonSaveFileExtension = ".json";

        //Ensures there will only be one playerstate per game

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnDestroy() {
        if (ActiveSave != null) {
            SaveGame();
        }
    }

    /**
     * Saves the currently active save file to the currently active save slot
     * @note if UseJsonSaves is enabled, a binary save will still be generated
     */
    public void SaveGame() 
    {
        if (UseJsonSaves) {
            SaveJson();
        }

        SaveBinary();

        Debug.Log("Game Saved");
    }

    /**
     * Loads save file in specified save slot
     * @param saveSlot the slot to load
     * @note if the slot empty, LoadGame will create and activate a new save
     */
    public void LoadGame(int saveSlot) 
    {
        SaveSlot = saveSlot;

        if (UseJsonSaves) {
            LoadJson(saveSlot);
        }
        else {
            LoadBinary(saveSlot);
        }

        Debug.Log("Game Loaded");
    }

    /**
     * Allows caller to query if a save file has been created for the save slot
     * @param saveSlot the slot to query
     * @return Whether or not the slot has a save file
     */
    public bool SlotHasSave(int saveSlot) 
    {
        string saveFilePath =
            SaveDirectory +
            BinarySaveFileName +
            saveSlot.ToString() +
            BinarySaveFileExtension;

        // If JSON saves are not in use, existence of binary file is sufficient proof
        if (!UseJsonSaves) {
            return File.Exists(saveFilePath);
        }
        else {
            string jsonSaveFilePath =
                SaveDirectory +
                JsonSaveFileName +
                saveSlot.ToString() +
                JsonSaveFileExtension;

            // Binary files are always generated on save.
            return File.Exists(saveFilePath) && File.Exists(jsonSaveFilePath);
        }
    }

    /**
     * Destroys save file in specified slot
     * @param slot The save slot to destroy
     * @note Will destroy both JSON an Binary save files 
     *       associated with the target slot
     */
    public void EraseSave(int targetSlot) 
    {
        string binaryFilePath =
            SaveDirectory +
            BinarySaveFileName +
            targetSlot.ToString() +
            BinarySaveFileExtension;

        if (File.Exists(binaryFilePath)) {
            Debug.Log("Erasing binary save: " + binaryFilePath);
            File.Delete(binaryFilePath);
        }

        string jsonFilePath =
            SaveDirectory +
            JsonSaveFileName +
            targetSlot.ToString() +
            JsonSaveFileExtension;

        if (File.Exists(jsonFilePath)) {
            Debug.Log("Erasing JSON save: " + jsonFilePath);
            File.Delete(jsonFilePath);
        }
    }

    /**
     * Used to cache puzzle state when exiting puzzle scene
     * @param puzzleName the name of the puzzle being cached
     * @param solutionPanel PuzzleElement containing player instructions
     */
    public void SavePuzzleSolution(string puzzleName, GameObject solutionPanel, GameObject cardPlayedPanel) 
    {
        PuzzleSave puzzleData;
        if (!ActiveSave.PuzzleSaveDictionary.ContainsKey(puzzleName)) {
            ActiveSave.PuzzleSaveDictionary[puzzleName] = new PuzzleSave(puzzleName);
        }

        puzzleData = ActiveSave.PuzzleSaveDictionary[puzzleName];
        
        // Reset solution.
        puzzleData.CachedInstructions = new List<CachedCommand>();

        // Save solution.
        if (solutionPanel != null)
        {
            var solution = new List<Command>(solutionPanel.GetComponentsInChildren<Command>());

            foreach (var command in solution)
            {
                puzzleData.CachedInstructions.Add(new CachedCommand(command.Instruction, System.Convert.ToInt32(command.Arg)));
            }
        }

        // Reset card choices.
        puzzleData.CachedCards = new List<CachedCard>();

        // Save player card choices.
        if (cardPlayedPanel != null)
        {
            foreach (Transform cardObj in cardPlayedPanel.transform)
            {
                string type = cardObj.name.Split('x')[0];
                puzzleData.CachedCards.Add(new CachedCard(type));
            }
        }

    }

    /**
     * Allows caller to request the score for the active save.
     * @return The score associated with the active save.
     */
    public int GetScore() {
        return ActiveSave.PlayerScore;
    }

    /**
     * Allows caller to update the score for the active save.
     * @param number to add to the score
     */
    public void AddToScore(int addend) {
        ActiveSave.PlayerScore += addend;
    }

    /**
     * Allows caller to request if a puzzle save exists on the active save.
     * @param name of puzzle to check
     * @return boolean representing if the save was found
     */
    public bool ContainsPuzzleSave(string puzzleName) {
        return ActiveSave.PuzzleSaveDictionary.ContainsKey(puzzleName);
    }

    /**
    * Allows caller to add a PuzzleSave object to the active save's
    * PuzzleSaveDictionary.
    * @param name of puzzle to add
    */
    public void AddPuzzleSave(string puzzleName) {
        ActiveSave.AddPuzzleSave(new PuzzleSave(puzzleName));
    }

    /**
     * Marks specified puzzle as complete
     * @param puzzleName name of the puzzle to mark complete
     */
    public void MarkPuzzleCompleted(string puzzleName) {
        ActiveSave.PuzzleSaveDictionary[puzzleName].Completed = true;
    }

    /**
     * Allows caller to record the event of earning a level star
     * @param puzzleName the puzzle the player earned the star for
     * @param star the type of star earned
     */
    public void EarnStar(string puzzleName, StarType star) {
        if (star == StarType.EFFICIENCY) {
            ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedEfficiencyStar = true;
        }
        else if (star == StarType.INSTRUCTION_COUNT) {
            ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedInstructionCountStar = true;
        }
        else if (star == StarType.MEMORY) {
            ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedMemoryStar = true;
        }
        else {
            throw new System.NotImplementedException();
        }
    }

    /**
     * Allows caller to request a cached solution for the specified puzzle
     * @param puzzleName puzzle for which solution is being requested
     * @return A cached list of commands which can be used to spawn the appopriate gameobjects
     */
    public List<CachedCommand> GetCachedSolution(string puzzleName) {
        return ActiveSave.PuzzleSaveDictionary[puzzleName].CachedInstructions;
    }

    /**
     * Allows caller to request the cached cards for the specified puzzle
     * @param puzzleName puzzle for which solution is being requested
     * @return A cached list of cards which can be used to spawn the appopriate gameobjects
     */
    public List<CachedCard> GetCachedCards(string puzzleName) {
        return ActiveSave.PuzzleSaveDictionary[puzzleName].CachedCards;
    }

    /**
     * Allows caller to query if puzzle is completed
     * @param puzzleName the puzzle to query for
     */
    public bool GetPuzzleCompleted(string puzzleName) {
        return 
            ActiveSave.PuzzleSaveDictionary.ContainsKey(puzzleName) && 
            ActiveSave.PuzzleSaveDictionary[puzzleName].Completed;
    }

    /**
     * Allows caller to query if the star specified was earned for the puzzle
     * @param puzzleName the puzzle to query in
     * @param star the star being queried about
     * @return Is star earned
     */
    public bool GetStarEarned(string puzzleName, StarType star) {
        if (star == StarType.EFFICIENCY) {
            return ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedEfficiencyStar;
        }
        else if (star == StarType.INSTRUCTION_COUNT) {
            return ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedInstructionCountStar;
        }
        else if (star == StarType.MEMORY) {
            return ActiveSave.PuzzleSaveDictionary[puzzleName].EarnedMemoryStar;
        }
        else {
            throw new System.NotImplementedException();
        }
    }

    /** Helper function to write save to binary */
    private void SaveBinary() 
    {
        string filePath =
            SaveDirectory +
            BinarySaveFileName +
            SaveSlot.ToString() +
            BinarySaveFileExtension;

        Debug.Log("Writing binary save file to: " + filePath);

        BinaryFormatter saveFileWriter = new BinaryFormatter();
        FileStream saveFile = File.Create(filePath);
        saveFileWriter.Serialize(saveFile, ActiveSave);
        saveFile.Close();

    }

    /** Helper function to write save to json */
    private void SaveJson() 
    {
        string filePath =
            SaveDirectory +
            JsonSaveFileName +
            SaveSlot.ToString() +
            JsonSaveFileExtension;

        Debug.Log("Writing JSON save file to: " + filePath);
        string jsonSave = JsonUtility.ToJson(ActiveSave);

        StreamWriter saveFile = new StreamWriter(filePath, false);

        saveFile.WriteLine(jsonSave);
        saveFile.Close();
    }

    /** Helper function to load save from binary */
    private void LoadBinary(int slot) 
    {
        // Construct filepath
        string filePath =
            SaveDirectory +
            BinarySaveFileName +
            slot.ToString() +
            BinarySaveFileExtension;

        Debug.Log("Loading binary save from: " + filePath);

        // Check for save file existence
        if (!File.Exists(filePath)) {
            ActiveSave = new Save();
            return;
        }

        // Load file
        BinaryFormatter saveFileLoader = new BinaryFormatter();
        FileStream save = File.Open(filePath, FileMode.Open);
        Save loadedSave = (Save)saveFileLoader.Deserialize(save);
        save.Close();

        // Make save file active
        ActiveSave = loadedSave;
    }

    /** Helper function to load save from json */
    private void LoadJson(int slot) 
    {
        // Construct filepath
        string filePath = 
            SaveDirectory + 
            JsonSaveFileName + 
            slot.ToString() + 
            JsonSaveFileExtension;
        
        Debug.Log("Loading json save from: " + filePath);

        // Check for save file existenc
        if (!File.Exists(filePath)) {
            ActiveSave = new Save();
            return;
        }

        // Load file
        string json = File.ReadAllText(filePath);
        Save loadedSave = JsonUtility.FromJson<Save>(json);

        // Make save file active
        ActiveSave = loadedSave;
    }

}
