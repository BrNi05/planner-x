using System; using System.Collections.Generic; using System.Linq; using UnityEngine; using UnityEngine.UI; using UnityEngine.EventSystems;

public class Displayer : MonoBehaviour
{
    public static UIController UIController; public static IOHandler IOHandler;
    static public List<string> EntryList = new List<string>(); static public List<string> notDisplayedEntries = new List<string>(); // entry lists
    static byte displayMode; static ushort numOfDisplayedEntries; public static DateTime now; // generally used
    static bool toRun = true; // used to prevent reloading and future loading at the same time
    public static bool futureEntryDisplay = false; // stores if futureEntries are being displayed/ should displayed


    private void Start() { now = DateTime.Now; LoadEntries(); }

    void FixedUpdate() { now = DateTime.Now; if (toRun) { foreach (string item in notDisplayedEntries) { if (DateTime.Parse(IOHandler.FileRead_entry(item)[1]) < now) { ReloadEntries(); UIController.errorMessagePanel.SetActive(true); UIController.errorHeader.text = "ALERT"; UIController.errorMessage.text = "Entries have been reloaded."; break; } } } }

    public static void LoadEntries()
    {
        if (toRun)
        {
            toRun = false; displayMode = Convert.ToByte(IOHandler.PPGetInt("displayMode")); numOfDisplayedEntries = 0; EntryList.Clear(); notDisplayedEntries.Clear();
            foreach (string categoryName in UIController.categories)
            {
                IOrderedEnumerable<string> filesDir = displayMode switch
                {
                    0 => IOHandler.GetEntries_nameDes(categoryName),
                    1 => IOHandler.GetEntries_nameAsc(categoryName),
                    2 => IOHandler.GetEntries_dateDes(categoryName),
                    3 => IOHandler.GetEntries_dateAsc(categoryName),
                    _ => null,
                };
                foreach (string fileName in filesDir)
                {
                    if (DateTime.Parse(IOHandler.FileRead_entry(fileName)[1]) > now) { notDisplayedEntries.Add(fileName); EntryList.Remove(fileName); } else { EntryList.Add(fileName); }
                }
                if (!futureEntryDisplay) { Display(categoryName, Convert.ToUInt16(EntryList.Count())); } else { Display(categoryName, Convert.ToUInt16(notDisplayedEntries.Count())); }
            }
            toRun = true; UIController.CheckEntryCount(futureEntryDisplay ? (ushort)notDisplayedEntries.Count : (ushort)EntryList.Count);
        }
    }

    static void Display(string categoryName, ushort loadedCount)
    {
        if (loadedCount != 0)
        {
            for (ushort i = Convert.ToUInt16(numOfDisplayedEntries); i < loadedCount; i++)
            {
                string[] linesInFile; string textToFormat; Color textColor = new Color(1, 1, 1); FontStyle fontType = FontStyle.Normal;

                if (!futureEntryDisplay) { linesInFile = IOHandler.FileRead_entry(EntryList[loadedCount - (loadedCount - i)]); textToFormat = EntryList[i].Substring((EntryList[i].IndexOf('~') + 1)); }
                else { linesInFile = IOHandler.FileRead_entry(notDisplayedEntries[loadedCount - (loadedCount - i)]); textToFormat = notDisplayedEntries[i].Substring(notDisplayedEntries[i].IndexOf('~') + 1); }

                if (DateTime.Parse(linesInFile[2]) <= now) { textColor = new Color(1, 0, 0); }

                if (Convert.ToByte(linesInFile[0]) == 2) { fontType = FontStyle.Bold; }

                if (DateTime.Parse(linesInFile[2]) - now <= TimeSpan.FromDays(3) && DateTime.Parse(linesInFile[2]) - now >= TimeSpan.FromDays(0)) { if (fontType == FontStyle.Bold) { fontType = FontStyle.BoldAndItalic; } else { fontType = FontStyle.Italic; } }

                GameObject instance = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Entry"), StaticVariables.grid, instantiateInWorldSpace: false);
                instance.transform.GetChild(0).GetComponent<Text>().color = textColor; instance.transform.GetChild(1).GetComponent<Text>().color = textColor;
                instance.transform.GetChild(0).GetComponent<Text>().fontStyle = fontType; instance.transform.GetChild(1).GetComponent<Text>().fontStyle = fontType;
                instance.GetComponent<Button>().onClick.AddListener(delegate { UIController.EntrySelected(instance.transform.GetChild(0).GetComponent<Text>().text, instance.transform.GetChild(1).GetComponent<Text>().text); });
                instance.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UIController.DeleteEntry((ushort)instance.transform.GetSiblingIndex()); });
                EventTrigger trigger = instance.AddComponent<EventTrigger>();
                var pointerHover = new EventTrigger.Entry(); var pointerExit = new EventTrigger.Entry(); var pointerClick = new EventTrigger.Entry();
                pointerHover.eventID = EventTriggerType.PointerEnter; pointerExit.eventID = EventTriggerType.PointerExit; pointerClick.eventID = EventTriggerType.PointerClick;
                pointerHover.callback.AddListener(delegate { UIController.DeleteButtonDisplay((ushort)instance.transform.GetSiblingIndex()); }); pointerExit.callback.AddListener(delegate { UIController.DeleteButtonHide((ushort)instance.transform.GetSiblingIndex()); }); pointerClick.callback.AddListener(delegate { UIController.DeleteButtonHide((ushort)instance.transform.GetSiblingIndex()); } );
                trigger.triggers.Add(pointerHover); trigger.triggers.Add(pointerExit); trigger.triggers.Add(pointerClick);
                new EntryButton(categoryName, textToFormat, instance);

                numOfDisplayedEntries++;
            }
        }
    }

    public static void ReloadEntries() { StaticMethods.ClearScreen(StaticVariables.grid); LoadEntries(); }

    public void DisplayFutureEntries()
    {
        if (!futureEntryDisplay) { futureEntryDisplay = true; UIController.sideBarImage2.sprite = UIController.futureBackImage; ReloadEntries(); }
        else { futureEntryDisplay = false; UIController.sideBarImage2.sprite = UIController.intoFutureImage; ReloadEntries(); }
    }
}

public class EntryButton
{
    Text category; Text name;
    public string CategoryText { get { return category.text; } set { category.text = value; } }
    public string NameText { get { return name.text; } set { name.text = value; } }

    public EntryButton(string categoryText, string nameText, GameObject instance) { instance.transform.SetParent(StaticVariables.grid); category = instance.transform.GetChild(0).GetComponent<Text>(); name = instance.transform.GetChild(1).GetComponent<Text>(); CategoryText = categoryText; NameText = nameText; }
}

public class CatReorderButton
{
    Text catNameText;
    public string CatName { get { return catNameText.text; } set { catNameText.text = value; } }

    public CatReorderButton(string catName, GameObject instance) { instance.transform.SetParent(StaticVariables.catReorderGrid); catNameText = instance.transform.GetChild(0).GetComponent<Text>(); CatName = catName; }
}

public class StaticMethods : MonoBehaviour
{
    public static void ClearScreen(Transform aGrid = null) { Transform gridTransform = (aGrid == null ? StaticVariables.grid : aGrid); for (ushort i = 0; i < gridTransform.childCount; i++) { Destroy(gridTransform.GetChild(i).gameObject); } }
}

public static class StaticVariables { public static Transform grid; public static Transform catReorderGrid; }