using System; using System.IO; using System.Linq; using UnityEngine;

public class IOHandler : MonoBehaviour
{
    static public string username = Environment.UserName;
    static public string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    static public string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Planner X" + @"\Entries";
    //string exePath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName + @"\Planner X.exe";

    private void Awake() { Displayer.IOHandler = this; }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public IOrderedEnumerable<string> GetEntries_nameAsc(string categoryName) { return Directory.EnumerateFiles(path, categoryName + "~*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).OrderBy(d => d); }
    public IOrderedEnumerable<string> GetEntries_nameDes(string categoryName) { return Directory.EnumerateFiles(path, categoryName + "~*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).OrderByDescending(d => d); }
    public IOrderedEnumerable<string> GetEntries_dateAsc(string categoryName) { return Directory.EnumerateFiles(path, categoryName + "~*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).OrderBy(d => File.GetCreationTimeUtc($@"C:\Users\{username}\Documents\Planner X\Entries\{d}.txt")); }
    public IOrderedEnumerable<string> GetEntries_dateDes(string categoryName) { return Directory.EnumerateFiles(path, categoryName + "~*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension).OrderByDescending(d => File.GetCreationTimeUtc($@"C:\Users\{username}\Documents\Planner X\Entries\{d}.txt")); }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void FileSysCheck()
    {
        Directory.CreateDirectory($@"C:\Users\{username}\Documents\Planner X"); Directory.CreateDirectory($@"C:\Users\{username}\Documents\Planner X\Entries");
        if (!File.Exists($@"C:\Users\{username}\Documents\Planner X\categories.txt")) { StreamWriter sw = File.CreateText($@"C:\Users\{username}\Documents\Planner X\categories.txt"); sw.WriteLine("Personal"); sw.WriteLine("Work"); sw.WriteLine("Travel"); sw.WriteLine("Others"); sw.Flush(); sw.Close(); }
        if (!PPHasKey("displayMode")) { PPSet("displayMode", 1); } if (!PPHasKey("colorScheme")) { PPSet("colorScheme", 0); }
        if (!PPHasKey("targetFPS")) { PPSet("targetFPS", 60); }
    }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CreateEntryData(ushort categoryIndex, string entryNameInput, ushort importanceChoice, string startDateInput, string deadlineInput, string notesInput, bool overwriteCT)
    {
        StreamWriter sw = File.CreateText($@"C:\Users\{username}\Documents\Planner X\Entries\{UIController.categoriesList[categoryIndex]}~{entryNameInput}.txt");
        sw.WriteLine(importanceChoice); sw.WriteLine(startDateInput); sw.WriteLine(deadlineInput); sw.WriteLine(notesInput); sw.Flush(); sw.Close();
        if (overwriteCT) { File.SetCreationTimeUtc($@"C:\Users\{username}\Documents\Planner X\Entries\{UIController.categoriesList[categoryIndex]}~{entryNameInput}.txt", UIController.creationDate); }
    }

    public string[] FileRead_cat() { return File.ReadAllLines($@"C:\Users\{username}\Documents\Planner X\categories.txt"); }

    public string[] FileRead_entry() { return File.ReadAllLines($@"C:\Users\{username}\Documents\Planner X\Entries\{UIController.selectedEntryCategory}~{UIController.selectedEntry}.txt"); }

    public string[] FileRead_entry(string name) { return File.ReadAllLines($@"C:\Users\{username}\Documents\Planner X\Entries\{name}.txt"); }

    public bool FileExists(string path) { return File.Exists(path); }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void EncryptFile(string pathExt) { /* File.Encrypt($@"C:\Users\{username}\Documents\Planner X\Entries\{pathExt}.txt"); */ }

    public void DecryptFile(string pathExt) { /* File.Decrypt($@"C:\Users\{username}\Documents\Planner X\Entries\{pathExt}.txt"); */ }

// Settings -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CreateShortcut(bool onDesktop)
    {
        string target = onDesktop ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Planner X.url" : $@"C:\Users\{username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Planner X.url";
        using StreamWriter writer = new StreamWriter(target); writer.WriteLine("[InternetShortcut]"); writer.WriteLine(@"URL=file:///" + @"C:\Program Files\Planner X\Planner X.exe"); writer.WriteLine("IconIndex=265"); writer.WriteLine(@"IconFile=C:\WINDOWS\System32\SHELL32.dll");
        writer.Flush(); writer.Close();
    }

    public void DeleteCatIO()
    {
        File.WriteAllText($@"C:\Users\{username}\Documents\Planner X\categories.txt", string.Empty);
        StreamWriter sw = new StreamWriter($@"C:\Users\{username}\Documents\Planner X\categories.txt"); foreach (string item in UIController.categoriesList) { sw.WriteLine(item); }
        sw.Flush(); sw.Close();
    }

    public void AddCatIO()
    {
        File.WriteAllText($@"C:\Users\{username}\Documents\Planner X\categories.txt", string.Empty);
        StreamWriter sw = new StreamWriter($@"C:\Users\{username}\Documents\Planner X\categories.txt"); foreach (string item in UIController.categoriesList) { sw.WriteLine(item); }
        sw.Flush(); sw.Close();
    }

    public void SaveCatIO()
    {
        File.WriteAllText($@"C:\Users\{username}\Documents\Planner X\categories.txt", string.Empty);
        StreamWriter sw = new StreamWriter($@"C:\Users\{username}\Documents\Planner X\categories.txt"); foreach (string item in UIController.tempCategoriesList) { sw.WriteLine(item); } sw.Flush(); sw.Close();
    }

    public IOrderedEnumerable<string> FilesInDir() { return Directory.EnumerateFiles(path).Select(Path.GetFileName).OrderBy(p => p); }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void DeleteFile(string path, bool valueToCT) { if (valueToCT) { UIController.creationDate = File.GetCreationTimeUtc(path); } File.Delete(path); }

    public void DelPXDir() { DelSec(); Directory.Delete($@"C:\Users\{username}\Documents\Planner X", true); PlayerPrefs.DeleteAll(); }

    public void DelSec() { UIController.securityOn = false; PlayerPrefs.DeleteKey("password"); PlayerPrefs.DeleteKey("secQ1Index"); PlayerPrefs.DeleteKey("secQ2Index"); PlayerPrefs.DeleteKey("secQ3Index"); PlayerPrefs.DeleteKey("secQ1A"); PlayerPrefs.DeleteKey("secQ2A"); PlayerPrefs.DeleteKey("secQ3A"); }

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void PPSet(string key, int value) { PlayerPrefs.SetInt(key, value); }

    public void PPSet(string key, string value){  PlayerPrefs.SetString(key, value); }

    public void PPSet(string key, float value) { PlayerPrefs.SetFloat(key, value); }

    public int PPGetInt(string key) { return PlayerPrefs.GetInt(key, -1); }

    public string PPGetString(string key) { return PlayerPrefs.GetString(key, ""); }

    public float PPGetFloat(string key) { return PlayerPrefs.GetFloat(key, -1); }

    public bool PPHasKey(string key) { return PlayerPrefs.HasKey(key); }
}

public class EncryptionHandler : MonoBehaviour
{

}
