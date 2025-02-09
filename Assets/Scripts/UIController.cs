using System; using System.Collections.Generic; using System.Text.RegularExpressions; using UnityEngine; using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Grid references")]
    public Transform grid;
    public Transform catReorderGrid;

    [Header("Panel references")]
    public GameObject mainMenuPanel;
    public GameObject addTaskPanel;
    public GameObject sidebarPanel;
    public GameObject upperbarPanel;
    public GameObject infosPanel;
    public GameObject settingsPanel;
    public GameObject securitySetupPanel;
    public GameObject securityOnPanel;
    public GameObject errorMessagePanel;
    public GameObject signInPanel;
    public GameObject accountRecoveryPanel;

    [Header("Sidebar References")]
    public Image sideBarImage2;
    public Sprite futureBackImage;
    public Sprite intoFutureImage;

    [Header("TitleAndBackUpperPanel References")]
    public Text titleText;
    public GameObject PX_logo;
    public GameObject barCatText;
    public GameObject barTitleText;

    [Header("AddTask References")]
    public InputField entryName;
    public Dropdown catDropdown;
    public Dropdown importanceDropdown;
    public InputField startDate; public Text startDatePlaceholder;
    public InputField deadline; public Text deadlinePlaceholder;
    public InputField notes; public Text notesPlaceholder;

    public GameObject calendarPanel;
    public Text dayText;
    public Text monthText;
    public Text yearText;
    public Text hourText;
    public Text minText;
    public Text secText;

    [Header("Informations reference")]
    public Scrollbar infosScrollBar;
    public Text externalText;

    [Header("Settings references")]
    public Scrollbar settingsScrollBar;
    public Toggle shortcut;
    public Toggle autostart;
    public Dropdown deleteCategoryDropdown;
    public InputField addCategoryInput;
    public Dropdown colorSchemeDropdown;
    public Dropdown displayModeDropdown;
    public GameObject hiderPanel;
    public GameObject catReorderPanel;
    public Slider fpsScroll;
    public Text fpsText;

    [Header("Error message text")]
    public Text errorHeader;
    public Text errorMessage;

    [Header("Sign in password reference")]
    public InputField psw;
    public Sprite notVisibleSprite;
    public Sprite visibleSprite;
    public Image passwordVisualizer;

    [Header("Account recovery references")]
    public Text secQ1; public InputField secQ1A;
    public Text secQ2; public InputField secQ2A;
    public Text secQ3; public InputField secQ3A;

    [Header("Security setup references")]
    public InputField pswSetupInput;
    public InputField pswSetupConfInput;
    public Dropdown secQ1Dropdown;
    public Dropdown secQ2Dropdown;
    public Dropdown secQ3Dropdown;
    public InputField secQ1AInput;
    public InputField secQ2AInput;
    public InputField secQ3AInput;

    [Header("Security setup references")]
    public Toggle localEncryptionToggle;

    // general variables -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Other script references")]
    public IOHandler IOHandler;
    public UIAnimation UIAnim;

    static public string[] categories; static public List<string> categoriesList; static public List<string> tempCategoriesList; // categories
    static public string selectedEntry, selectedEntryCategory; ushort selectedButton = 0; // used for main navi
    static bool taskModifyIntention = false; // marks if user is in task details
    bool detectKeys_catReorder = false; bool detectArrowKeys = true; // used for catReorder key detection (to only 1 trigger)
    public static bool callAddTask = false; // calls add task, as the call place is static and AddTask isn't
    bool detectRefreshKey = true; // used for F1 (CatReload) key detection (to only 1 trigger)
    public static bool securityOn; // determines (using PP and latter modifications if protection is on)
    byte pressedButton; // used for the calendar date saving - startDate or deadline

    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        IOHandler.FileSysCheck(); securityOn = IOHandler.PPHasKey("password");
        Application.targetFrameRate = IOHandler.PPGetInt("targetFPS");

        Displayer.UIController = this; StaticVariables.grid = grid; StaticVariables.catReorderGrid = catReorderGrid;
        categories = IOHandler.FileRead_cat(); categoriesList = new List<string>(categories); tempCategoriesList = new List<string>(categoriesList);
    }
    void Start() { if (!securityOn) { PanelManager(mainMenu: true, sidebar: true); } else { PanelManager(signIn: true); } }
    void Update()
    {
        if (detectKeys_catReorder)
        {
            if (Input.GetKey(KeyCode.UpArrow) && detectArrowKeys)
            {
                detectArrowKeys = false;
                ushort prevMarkerPos = selectedButton;
                if (selectedButton-- == 0) { selectedButton = Convert.ToUInt16(tempCategoriesList.Count - 1); }
                string movePlace = tempCategoriesList[selectedButton];
                string sourcePlace = tempCategoriesList[prevMarkerPos];
                tempCategoriesList[selectedButton] = sourcePlace;
                tempCategoriesList[prevMarkerPos] = movePlace;
                StaticMethods.ClearScreen(catReorderGrid); CategoryReorder(selectedButton);
            }
            if (Input.GetKey(KeyCode.DownArrow) && detectArrowKeys)
            {
                detectArrowKeys = false;
                ushort prevMarkerPos = selectedButton;
                if (selectedButton++ == tempCategoriesList.Count - 1) { selectedButton = 0; }
                string sourcePlace = tempCategoriesList[prevMarkerPos];
                string movePlace = tempCategoriesList[selectedButton];
                tempCategoriesList[selectedButton] = sourcePlace;
                tempCategoriesList[prevMarkerPos] = movePlace;
                StaticMethods.ClearScreen(catReorderGrid); CategoryReorder(selectedButton);
            }
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) { detectArrowKeys = true; }
        }

        if (detectRefreshKey && Input.GetKey(KeyCode.F1)) { detectRefreshKey = false; Displayer.ReloadEntries(); }
        if (!Input.GetKey(KeyCode.F1)) { detectRefreshKey = true; }

        if (callAddTask) { AddTask(); callAddTask = false; }
    }

    // button binds - MainMenu -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CheckEntryCount(ushort listCount) { if (listCount == 0) { PX_logo.SetActive(true); barCatText.SetActive(false); barTitleText.SetActive(false); } else { PX_logo.SetActive(false); barCatText.SetActive(true); barTitleText.SetActive(true); } }

    public void BackButton()
    {
        if (addTaskPanel.activeInHierarchy || infosPanel.activeInHierarchy || (settingsPanel.activeInHierarchy && hiderPanel.activeInHierarchy != true)) { PanelManager(mainMenu: true, sidebar: true); }
        if (hiderPanel.activeInHierarchy) { SaveCatOrder(false); }
        if (securityOnPanel.activeInHierarchy) { PanelManager(settings: true, sidebar: true, upperBar: true); titleText.text = "Settings"; }
        if (securitySetupPanel.activeInHierarchy && !securityOn) { PanelManager(settings: true, sidebar: true, upperBar: true); titleText.text = "Settings"; }
        if (securitySetupPanel.activeInHierarchy && securityOn) { PanelManager(securityOn: true, sidebar: true, upperBar: true); titleText.text = "Security Settings"; }
    }

    static public void EntrySelected(string category, string name) { selectedEntry = name; selectedEntryCategory = category; taskModifyIntention = true; callAddTask = true; }

    public void DeleteButtonDisplay(ushort entryNum) { grid.transform.GetChild(entryNum).GetChild(2).gameObject.SetActive(true); }

    public void DeleteButtonHide(ushort entryNum) { grid.transform.GetChild(entryNum).GetChild(2).gameObject.SetActive(false); }

    public void DeleteEntry(ushort fileNum)
    {
        string toDelete = !Displayer.futureEntryDisplay ? Displayer.EntryList[fileNum] : Displayer.notDisplayedEntries[fileNum];
        IOHandler.DeleteFile($@"C:\Users\{IOHandler.username}\Documents\Planner X\Entries\{toDelete}.txt", false);
        Displayer.EntryList.Clear(); Displayer.notDisplayedEntries.Clear(); PanelManager(mainMenu: true, sidebar: true); StaticMethods.ClearScreen(); Displayer.LoadEntries();
    }

    public void AddTask()
    {
        PanelManager(addTask: true, sidebar: true, upperBar: true); titleText.text = "Add Entry"; if (!callAddTask) { taskModifyIntention = false; } calendarPanel.SetActive(false);
        catDropdown.ClearOptions(); catDropdown.AddOptions(categoriesList); catDropdown.RefreshShownValue(); entryName.text = null; catDropdown.value = 0; importanceDropdown.value = 0; startDate.text = null; deadline.text = null; notes.text = null;

        if (!taskModifyIntention) { startDatePlaceholder.text = DateTime.Now.ToString("d"); deadlinePlaceholder.text = DateTime.Today.AddDays(365).ToString("d"); notesPlaceholder.text = "Enter your notes here..."; }
        else
        {
            titleText.text = "Modify an entry";
            string[] fileContent = IOHandler.FileRead_entry();
            entryName.text = selectedEntry; catDropdown.value = categoriesList.IndexOf(selectedEntryCategory); importanceDropdown.value = Convert.ToInt32(fileContent[0]) - 1; startDate.text = fileContent[1]; deadline.text = fileContent[2]; notes.text = fileContent[3];
            if (fileContent[3] == null || fileContent[3] == "") { notesPlaceholder.text = "Enter your notes here..."; }
        }
    }

    public void Lock() { if (securityOn) { PanelManager(signIn: true); psw.contentType = InputField.ContentType.Password; } }

    public void Settings()
    {
        PanelManager(settings: true, sidebar: true, upperBar: true); titleText.text = "Settings"; settingsScrollBar.value = 1; hiderPanel.SetActive(false); catReorderPanel.SetActive(false);

        deleteCategoryDropdown.ClearOptions(); deleteCategoryDropdown.AddOptions(categoriesList); deleteCategoryDropdown.RefreshShownValue();
        shortcut.isOn = IOHandler.FileExists(IOHandler.desktopDir + @"\Planner X.url");
        autostart.isOn = IOHandler.FileExists($@"C:\Users\{IOHandler.username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Planner X.url");
        colorSchemeDropdown.value = IOHandler.PPGetInt("colorScheme");
        displayModeDropdown.value = IOHandler.PPGetInt("displayMode");
        fpsScroll.value = IOHandler.PPHasKey("targetFPS") ? IOHandler.PPGetInt("targetFPS") : 60; fpsText.text = fpsScroll.value.ToString();
    }

    public void Infos() { PanelManager(infos: true, sidebar: true, upperBar: true); titleText.text = "Informations"; infosScrollBar.value = 1; externalText.text = null; }

    // button binds - AddTask -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public static DateTime creationDate;
    public void SaveTask()
    {
        bool saveable = true;
        string[] entryFileContent = new string[4] { "", "", "", "" }; if (taskModifyIntention) { Array.Clear(entryFileContent, 0, entryFileContent.Length); entryFileContent = IOHandler.FileRead_entry(); }

        string entryNameInput = entryName.text.Trim();
        if (entryNameInput == "" || entryNameInput == null) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text = "The entry does not have a name!"; }
        if (entryNameInput.Contains("~")) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text = "The entry's name contains forbidden characters!"; }

        byte categoryIndex = Convert.ToByte(catDropdown.value); if (categories.Length == 0) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text += "\nThere are no categories existing!"; }

        byte importanceChoice = Convert.ToByte(importanceDropdown.value + 1);

        string startDateInput = startDate.text.Trim();
        if (startDateInput != "" && startDateInput != null) { try { DateTime.Parse(startDateInput); } catch (Exception) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text += "\nThe input is not a valid date!"; } }
        else if (startDateInput == "") { startDateInput = DateTime.Now.ToString("d"); }

        string deadlineInput = deadline.text.Trim();
        if (deadlineInput != "" && deadlineInput != null) { try { DateTime.Parse(deadlineInput); } catch (Exception) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text += "\nThe input is not a valid date!"; } }
        else if (deadlineInput == "" && saveable) { deadlineInput = DateTime.Parse(DateTime.Now.AddDays(+365).ToString("d")) < DateTime.Parse(startDateInput) ? DateTime.Parse(startDateInput).AddDays(365).ToString("d") : DateTime.Now.AddDays(+365).ToString("d"); }

        if (saveable && DateTime.Parse(startDateInput) > DateTime.Parse(deadlineInput)) { saveable = false; errorMessagePanel.SetActive(true); errorMessage.text += "\nStartdate - deadline conflicting!"; }

        string notesInput = notes.text.Trim();

        if (categories.Length != 0 && saveable)
        {
            if (!taskModifyIntention) { saveable = CheckExists(catDropdown.options[categoryIndex].text, entryNameInput); }
            else if (taskModifyIntention && (entryNameInput != selectedEntry || categoriesList[categoryIndex] != selectedEntryCategory)) { saveable = CheckExists(catDropdown.options[categoryIndex].text, entryNameInput); }
        }
        if (saveable && taskModifyIntention) { IOHandler.DeleteFile($@"C:\Users\{IOHandler.username}\Documents\Planner X\Entries\{selectedEntryCategory}~{selectedEntry}.txt", true); }
        if (saveable) { ReloadScreen(categoryIndex, entryNameInput, importanceChoice, startDateInput, deadlineInput, notesInput); }

        bool CheckExists(string category, string name) { if (IOHandler.FileExists($@"C:\Users\{IOHandler.username}\Documents\Planner X\Entries\{category}~{name}.txt")) { errorMessagePanel.SetActive(true); errorMessage.text += "\nAn entry with such a name and category exists!"; return false; } else { return true; } }
        void ReloadScreen(byte categoryIndex, string entryNameInput, byte importanceChoice, string startDateInput, string deadlineInput, string notesInput)
        {
            IOHandler.CreateEntryData(categoryIndex, entryNameInput, importanceChoice, startDateInput, deadlineInput, notesInput, taskModifyIntention);
            if (IOHandler.PPGetInt("encryptionStatus") == 1) { IOHandler.EncryptFile(categoriesList[categoryIndex] + "~" + entryNameInput); }
            errorMessage.text = null; StaticMethods.ClearScreen(); PanelManager(mainMenu: true, sidebar: true); Displayer.LoadEntries();
        }
    }
    DateTime toModDate;
    public void CalendarViewDisplay(int buttonNO)
    {
        calendarPanel.SetActive(true); pressedButton = (byte)buttonNO; bool successfulParse = true;
        toModDate = DateTime.Parse(Displayer.now.ToString("yyyy.MM.dd")); DateTime nowDate = Displayer.now;

        if (buttonNO == 0) { try { toModDate = DateTime.Parse(startDate.text); } catch (Exception) { successfulParse = false; } }
        else { try { toModDate = DateTime.Parse(deadline.text); } catch (Exception) { successfulParse = false; } }

        if (buttonNO == 1 && !successfulParse) { nowDate = DateTime.Parse(deadlinePlaceholder.text); toModDate = nowDate; }
        yearText.text = successfulParse ? toModDate.Year.ToString() : nowDate.Year.ToString();
        monthText.text = successfulParse ? toModDate.Month.ToString() : nowDate.Month.ToString();
        dayText.text = successfulParse ? toModDate.Day.ToString() : nowDate.Day.ToString();
        hourText.text = successfulParse ? toModDate.Hour.ToString() : "0";
        minText.text = successfulParse ? toModDate.Minute.ToString() : "0";
        secText.text = successfulParse ? toModDate.Second.ToString() : "0";
    }

    public void ExitCalendarView() { calendarPanel.SetActive(false); }

    public void SaveCalendarData()
    {
        string dateText = $"{yearText.text}.{monthText.text}.{dayText.text} {hourText.text}:{minText.text}:{secText.text}";
        if (pressedButton == 0) { startDate.text = dateText; } else { deadline.text = dateText; }
        ExitCalendarView();
    }

    public void ArrowController(int arrowIndex_arrowDir)
    {
        sbyte addition = 1;
        byte arrowIndex = (byte)arrowIndex_arrowDir; while (arrowIndex >= 10) { arrowIndex /= 10; }
        byte arrowDir = (byte)(arrowIndex_arrowDir - arrowIndex * 10); if (arrowDir == 0) { addition = -1; }
        DateTime newDate;
        
        if (arrowIndex == 1) { newDate = toModDate.AddYears(addition); }
        else if (arrowIndex == 2) { newDate = toModDate.AddMonths(addition); }
        else if (arrowIndex == 3) { newDate = toModDate.AddDays(addition); }
        else if (arrowIndex == 4) { newDate = toModDate.AddHours(addition); }
        else if (arrowIndex == 5) { newDate = toModDate.AddMinutes(addition * 5); }
        else { newDate = toModDate.AddSeconds(addition * 10); } // arrowIndex == 6

        yearText.text = newDate.Year.ToString(); monthText.text = newDate.Month.ToString(); dayText.text = newDate.Day.ToString(); hourText.text = newDate.Hour.ToString(); minText.text = newDate.Minute.ToString(); secText.text = newDate.Second.ToString();
        toModDate = newDate;
    }

    // button binds - Informations -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void Legal() { externalText.text = null; TextAsset mytxtData = (TextAsset)Resources.Load("texts/LICENSE"); externalText.text = mytxtData.text; }

    public void ChangeLog() { externalText.text = null; TextAsset mytxtData = (TextAsset)Resources.Load("texts/changeLog"); externalText.text = mytxtData.text; }

    // button binds - Settings -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SecPage()
    {
        if (!securityOn) { PanelManager(securitySetup: true, sidebar: true, upperBar: true); titleText.text = "Security Setup"; pswSetupInput.text = pswSetupConfInput.text = secQ1AInput.text = secQ2AInput.text = secQ3AInput.text = null; }
        else { SecurityOnPanelDisplay(); titleText.text = "Security Settings"; }
    }

    public void DesktopShortcutToggle (bool isOn) { if (!isOn) { IOHandler.DeleteFile(IOHandler.desktopDir + @"\Planner X.url", false); } else { IOHandler.CreateShortcut(true); } }

    public void AutoStartToggle (bool isOn) { if (!isOn) { IOHandler.DeleteFile($@"C:\Users\{IOHandler.username}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\Planner X.url", false); } else { IOHandler.CreateShortcut(false); } }

    public void DeleteCategory()
    {
        if (categoriesList.Count != 0) { categoriesList.Remove(categoriesList[deleteCategoryDropdown.value]); IOHandler.DeleteCatIO(); ReloadCategories(); }
        else { errorMessagePanel.SetActive(true); errorMessage.text = "There are no categories to be deleted!"; }
        selectedButton = 0;
    }

    public void AddCategory()
    {
        string newCatName = addCategoryInput.text.Trim();
        if (categoriesList.Contains(newCatName)) { errorMessagePanel.SetActive(true); errorMessage.text = "A category with such name is already existing!"; }
        else if (newCatName == "" || newCatName == null) { errorMessagePanel.SetActive(true); errorMessage.text = "The category has no name or consisting of only spaces!"; }
        else { categoriesList.Add(newCatName); IOHandler.AddCatIO(); ReloadCategories(); addCategoryInput.text = null; }
        selectedButton = 0;
    }

    public void CategoryReorder(int toMark)
    {
        hiderPanel.SetActive(true); catReorderPanel.SetActive(true); titleText.text = "Category Reorder";

        foreach (string item in tempCategoriesList)
        {
            GameObject instance = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/catReorderButton"), catReorderGrid, instantiateInWorldSpace: false);
            instance.transform.GetChild(0).GetComponent<Text>().text = item;
            instance.GetComponent<Button>().onClick.AddListener(delegate { ButtonSelector((ushort)instance.transform.GetSiblingIndex()); } );
            new CatReorderButton(item, instance);
        }
        for (ushort i = 0; i < catReorderGrid.childCount; i++) { if (catReorderGrid.GetChild(i).GetChild(0).GetComponent<Text>().text == tempCategoriesList[toMark]) { catReorderGrid.GetChild(i).GetComponent<Image>().color = new Color(0.149f, 0.149f, 0.149f); detectKeys_catReorder = true; } }
    }

    void ButtonSelector(ushort buttonIndex)
    {
        for (ushort i = 0; i < catReorderGrid.transform.childCount; i++) { catReorderGrid.GetChild(i).GetComponent<Image>().color = new Color(0.2196078f, 0.2196078f, 0.2196078f); }
        catReorderGrid.GetChild(buttonIndex).GetComponent<Image>().color = new Color(0.149f, 0.149f, 0.149f);
        selectedButton = buttonIndex;
    }

    public void SaveCatOrder(bool calledToSave)
    {
        detectKeys_catReorder = false; PanelManager(settings: true, sidebar: true, upperBar: true); titleText.text = "Settings"; hiderPanel.SetActive(false); catReorderPanel.SetActive(false); StaticMethods.ClearScreen(catReorderGrid);
        if (calledToSave) { StaticMethods.ClearScreen(grid); IOHandler.SaveCatIO(); } ReloadCategories();
    }

    public void ColorSchemeSelectButton() { IOHandler.PPSet("colorScheme", colorSchemeDropdown.value); } // itt kell majd a színeket átállítani

    public void DisplayModeSelectButton() { byte prevDPMode = (byte)IOHandler.PPGetInt("displayMode"); IOHandler.PPSet("displayMode", displayModeDropdown.value); if (prevDPMode != displayModeDropdown.value) { Displayer.ReloadEntries(); } }

    public void SetTargetFPS_text() { fpsText.text = Convert.ToInt32(fpsScroll.value).ToString(); }

    public void SetTargetFPS() { IOHandler.PPSet("targetFPS", (int)fpsScroll.value); Application.targetFrameRate = (int)fpsScroll.value; }

    // button binds - SecuritySetup -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SecurityOnPanelDisplay() { localEncryptionToggle.isOn = IOHandler.PPGetInt("encryptionStatus") == 1; PanelManager(securityOn: true, sidebar: true, upperBar: true); }
    
    public void SaveSec()
    {
        bool saveable = true; errorMessage.text = null; string pswInput = pswSetupInput.text; string pswConfInput = pswSetupConfInput.text;

        if (pswInput.ToLower().Contains("password") || pswInput.ToLower().Contains("123456") || pswInput.ToLower().Contains("qwertz") || pswInput.ToLower().Contains("qwerty") || pswInput.ToLower().Contains("football"))
        {
            errorMessagePanel.SetActive(true); errorMessage.text = "The password is too easy to figure out, avoid usual expressions!"; saveable = false;
        }
        if (pswInput == null || pswInput.Length < 8 || pswInput.Length > 20 || !Regex.IsMatch(pswInput, "[0-9]") || !Regex.IsMatch(pswInput, "[a-zA-Z]"))
        {
            errorMessagePanel.SetActive(true); errorMessage.text = "The password should be at least 8 and max. 20 characters and should include both upper- and lowercase characters and numerical characters!"; saveable = false;
        }
        if (pswInput != pswConfInput) { errorMessagePanel.SetActive(true); errorMessage.text += "\nThe two passwords does not match!"; saveable = false; }
        if (secQ1AInput.text.Trim() == "" || secQ1AInput.text == null || secQ2AInput.text.Trim() == "" || secQ2AInput.text == null || secQ3AInput.text.Trim() == "" || secQ3AInput.text == null)
        {
            errorMessagePanel.SetActive(true); errorMessage.text += "\nOne or more security questions are not answered!"; saveable = false;
        }
        if (saveable)
        {
            IOHandler.PPSet("password", pswInput); IOHandler.PPSet("secQ1Index", secQ1Dropdown.value); IOHandler.PPSet("secQ2Index", secQ2Dropdown.value); IOHandler.PPSet("secQ3Index", secQ3Dropdown.value); IOHandler.PPSet("secQ1A", secQ1AInput.text); IOHandler.PPSet("secQ2A", secQ2AInput.text); IOHandler.PPSet("secQ3A", secQ3AInput.text);
            securityOn = true; PanelManager(settings: true, sidebar: true, upperBar: true); titleText.text = "Settings"; Lock();
        }
    }

    // button binds - Security On Panel --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void LocalEncryptionToggle(bool toggleValue)
    {
        if (toggleValue) { foreach (string item in IOHandler.FilesInDir()) { IOHandler.EncryptFile(item); } IOHandler.PPSet("encryptionStatus", 1); }
        else { foreach (string item in IOHandler.FilesInDir()) { IOHandler.DecryptFile(item); } IOHandler.PPSet("encryptionStatus", 0); }
    }

    public void ModifySecurity()
    {
        PanelManager(securitySetup: true, sidebar: true, upperBar: true); titleText.text = "Security Setup";
        pswSetupInput.text = pswSetupConfInput.text = IOHandler.PPGetString("password"); secQ1Dropdown.value = IOHandler.PPGetInt("secQ1Index"); secQ2Dropdown.value = IOHandler.PPGetInt("secQ2Index"); secQ3Dropdown.value = IOHandler.PPGetInt("secQ3Index"); secQ1AInput.text = IOHandler.PPGetString("secQ1A"); secQ2AInput.text = IOHandler.PPGetString("secQ2A"); secQ3AInput.text = IOHandler.PPGetString("secQ3A");
    }

    public void DeleteSecurity() { IOHandler.DelSec(); PanelManager(mainMenu: true, sidebar: true); }

    // button binds - ErrorMessage -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CloseTab() { errorMessage.text = null; errorMessagePanel.SetActive(false); errorHeader.text = "ERROR"; }

    // button binds - SignIn -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void PasswordVisible()
    {
        string pswString = psw.text; psw.text = null;

        if (psw.contentType == InputField.ContentType.Standard) { psw.contentType = InputField.ContentType.Password; passwordVisualizer.sprite = notVisibleSprite; }
        else { psw.contentType = InputField.ContentType.Standard; passwordVisualizer.sprite = visibleSprite; }
        psw.text = pswString;
    }

    public void ForgottenPassword()
    {
        psw.text = null; secQ1A.text = null; secQ2A.text = null; secQ3A.text = null; PanelManager(accountRecovery: true);
        secQ1.text = "  " + secQ1Dropdown.options[IOHandler.PPGetInt("secQ1Index")].text;
        secQ2.text = "  " + secQ2Dropdown.options[IOHandler.PPGetInt("secQ2Index")].text;
        secQ3.text = "  " + secQ3Dropdown.options[IOHandler.PPGetInt("secQ3Index")].text;
    }

    byte pswTries = 5;
    public void PasswordCheck()
    {
        if (psw.text == IOHandler.PPGetString("password")) { psw.text = null; PanelManager(mainMenu: true, sidebar: true); }
        else
        {
            pswTries--; errorMessagePanel.SetActive(true); errorMessage.text = $"Incorrect password!\n{pswTries} tries left!";
            if (pswTries == 0) { pswTries = 5; IOHandler.DelPXDir(); IOHandler.FileSysCheck(); PanelManager(mainMenu: true, sidebar: true); errorMessagePanel.SetActive(true); errorHeader.text = "WARNING"; errorMessage.text = "Planner X has just resetted itself after 5 failed sign in tries!"; }
        }
    }

    // button binds - AccountRecovery -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    byte secTries = 5;
    public void CheckInput()
    {
        byte correctAnsweres = 0;
        if (secQ1A.text == IOHandler.PPGetString("secQ1A")) { correctAnsweres++; } if (secQ2A.text == IOHandler.PPGetString("secQ2A")) { correctAnsweres++; } if (secQ3A.text == IOHandler.PPGetString("secQ3A")) { correctAnsweres++; }
        if (correctAnsweres >= 2) { ModifySecurity(); } else { secTries--; errorMessagePanel.SetActive(true); errorMessage.text = $"Authentication failed!\n{secTries} tries left!"; }
        if (secTries == 0) { secTries = 5; IOHandler.DelPXDir(); IOHandler.FileSysCheck(); PanelManager(mainMenu: true, sidebar: true); errorMessagePanel.SetActive(true); errorHeader.text = "WARNING"; errorMessage.text = "Planner X has just resetted itself after 5 failed emergency reset tries!"; }
    }

    // Helper Methods Section
    private void PanelManager(bool mainMenu = false, bool addTask = false, bool sidebar = false, bool upperBar = false, bool infos = false, bool settings = false, bool securitySetup = false, bool securityOn = false, bool errorMsg = false, bool signIn = false, bool accountRecovery = false)
    {
        mainMenuPanel.SetActive(mainMenu); addTaskPanel.SetActive(addTask); sidebarPanel.SetActive(sidebar); upperbarPanel.SetActive(upperBar); infosPanel.SetActive(infos); settingsPanel.SetActive(settings); securitySetupPanel.SetActive(securitySetup); securityOnPanel.SetActive(securityOn); errorMessagePanel.SetActive(errorMsg); signInPanel.SetActive(signIn); accountRecoveryPanel.SetActive(accountRecovery);
    }

    public void ReloadCategories()
    {
        Array.Clear(categories, 0, categories.Length); categoriesList.Clear(); tempCategoriesList.Clear();
        categories = IOHandler.FileRead_cat(); categoriesList = new List<string>(categories); tempCategoriesList = new List<string>(categoriesList);
        deleteCategoryDropdown.ClearOptions(); deleteCategoryDropdown.AddOptions(categoriesList); deleteCategoryDropdown.RefreshShownValue();
    }
}