using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    // Manage all UI Components in the Lobby Scene
    // Activate UI Animations and set UI informations containing user data

    #region UI Components
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject backgroundBase;
    [SerializeField] private float backgroundMoveSpeed;
    [SerializeField] private GameObject currentFragment;
    [SerializeField] private GameObject nextFragment;   // Assigned after the user clicked next fragment
    [SerializeField] private GameObject partsUpgradeBase;
    [SerializeField] private Animation fragmentChangeAnim;
    [SerializeField] private Image panel;               // For Fade-out effect
    [SerializeField] private Image[] fragmentButtons;   // reference to 3 buttons at the bottom
    [SerializeField] private Text jemCount;
    [SerializeField] private Text highScore;

    [SerializeField] private Text selectedPartsName;
    [SerializeField] private Text selectedPartsDescription;
    [SerializeField] private GameObject selectedPartsImage;
    [SerializeField] private Text ability1Title;
    [SerializeField] private Text ability2Title;
    [SerializeField] private Text ability3Title;
    [SerializeField] private Text ability1Description;
    [SerializeField] private Text ability2Description;
    [SerializeField] private Text ability3Description;
    [SerializeField] private Text ability1UpgradeJem;
    [SerializeField] private Text ability2UpgradeJem;
    [SerializeField] private Text ability3UpgradeJem;
    #endregion

    #region Member Variables
    private Vector2 moveDir;
    private float moveX = 1;
    private float moveY = 1;
    private float xScreenHalfSize;
    private float yScreenHalfSize;
    private GameObject target;      // reference to the selected fragment
    private GameObject currentPart; // reference to the currently equiped part obj
    private bool isConvertingUi = false;
    private float currentTime = 0f;  
    private float fadeoutTime = 1f;
    private int selectedPartsIdx = -1;
    private string currentParts;
    int[] partsUpgradeInfo;
    private Dictionary<string, int> selectedParts = new Dictionary<string, int>()
    {
        {"Missile",0 },
        {"Laser",1 },
        {"Barrier",2 },
        {"Emp",3 }
    };
    private string[] partsDescriptions = new string[4]
    {   "Destroy all Enemies with\r\nLarge-Range Missiles!",
        "Nobody can Survive\r\nafter Merciless Laser Attack",
        "Barrier will Protects\r\nYour Weakest Part!",
        "Enemies couldn't\r\nGet Close to You!"};
    private string[,,] partsInfo = new string[4, 3, 2]
    {
        {{"Parts Damage","Increase Missile Damage +"},{ "Parts Speed","Increase Missile Speed +"},{ "Abilities","Increase Missile Range"} },
        {{"Parts Damage","Increase Laser Damage +"},{ "Parts Speed","Increase Laser Speed +"},{ "Abilities","Make Another Laser"} },
        {{"Parts Damage","Increase Barrier Damage +"},{ "Parts Speed","Increase Speed Reduction +"},{ "Abilities","Make Shield Initially"} },
        {{"Parts Damage","Increase Emp Damage +"},{ "Parts Speed","Reduce Cool-Time +"},{ "Abilities","Additional Knock-Back Effect"} }
    };
    #endregion

    #region For Debug
    [SerializeField] private InputField input;
    #endregion

    private void Start()
    {
        SetUi();
        yScreenHalfSize = Screen.height / 2;
        xScreenHalfSize = Screen.width / 2;
        moveDir = new Vector2(moveX, moveY);
        SoundManager.instance.PlayBGM("LobbyBGM");
    }

    private void Update()
    {
        MoveBackground();
    }

    // Set user data to the Text at the start of the game
    private void SetUi()
    {
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString()+" J";
        highScore.text = ": "+LocalDatabaseManager.instance.HighScore.ToString();
        currentPart= GameObject.Find("Parts" + LocalDatabaseManager.instance.CurrentParts);
        if (currentPart != null)
            currentPart.GetComponent<Animation>().Play("PartsUiEquipAnim");
    }

    private void MoveBackground()
    {
        float x = backgroundBase.GetComponent<RectTransform>().anchoredPosition.x;
        float y = backgroundBase.GetComponent<RectTransform>().anchoredPosition.y;

        if (x < -xScreenHalfSize)
            moveX = UnityEngine.Random.Range(0, 1f);
        else if (x > xScreenHalfSize)
            moveX = UnityEngine.Random.Range(-1f, 0);
        if (y < -yScreenHalfSize)
            moveY = UnityEngine.Random.Range(0, 1f);
        else if (y > yScreenHalfSize)
            moveY = UnityEngine.Random.Range(-1f, 0);

        moveDir.x = moveX;
        moveDir.y = moveY;
        backgroundBase.GetComponent<RectTransform>().anchoredPosition += backgroundMoveSpeed * Time.deltaTime * moveDir.normalized;
    }

    /// <summary>
    /// Make Fade-out effect when the 'Battle' button clicked
    /// </summary>
    public void ChangeScene()
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        StartCoroutine("FadeOut");
    }

    private void UiConvertingState()
    {
        isConvertingUi = !isConvertingUi;
    }

    IEnumerator FadeOut()
    {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        while (alpha.a < 1)
        {
            currentTime += Time.deltaTime / fadeoutTime;
            alpha.a = Mathf.Lerp(0, 1, currentTime);    // make smooth effect by modifying the alpha value using linear interpolation
            panel.color = alpha;
            yield return null;
        }
        SceneManager.LoadScene("InGameScene");
    }

    /// <summary>
    /// Converting Parts in Lobby Scene, Parts Fragment
    /// </summary>
    /// <param name="part"></param>
    public void EquipParts(GameObject part)
    {
        // Conduct converting while not exceptional situation
        if (part == null || currentPart==part || isConvertingUi)
            return;
        isConvertingUi = true;
        SoundManager.instance.PlaySFX("PartsEquipSound");

        // Converting Animation
        part.GetComponent<Animation>().Play("PartsUiEquipAnim");
        currentPart.GetComponent<Animation>().Play("PartsUiOffAnim");
        currentPart = part;

        // Update newly equiped part to the database manager
        LocalDatabaseManager.instance.CurrentParts = part.transform.name.Substring(5);
        LocalDatabaseManager.instance.SavePartsData();
        Invoke("UiConvertingState", 0.6f);
    }

    // Change the alpha value( 0.3 or 1) to Emphasize the selected fragment button
    private void ButtonAlphaChange(Image image, float a)
    {
        Color alpha = image.color;
        alpha.a = a;
        image.color = alpha;
    }

    /// <summary>
    /// Convet to each Fragment by setting their parents and activate the animation
    /// </summary>
    /// <param name="targetFragment"></param>
    public void OnClickFragmentChange(GameObject targetFragment)
    {
        // Do nothing if selecting current fragment or already converting fragments
        if (currentFragment.transform.GetChild(0) == targetFragment.transform || isConvertingUi)
            return;
        isConvertingUi = true;
        SoundManager.instance.PlaySFX("BasicButtonSound");

        // Set the alpha value of each fragment button(if selected, assign 1)
        foreach (Image i in fragmentButtons)
            ButtonAlphaChange(i, 0.3f);
        switch (targetFragment.transform.name)
        {
            case "TitleFragment":
                ButtonAlphaChange(fragmentButtons[0], 1f);
                break;
            case "PartsFragment":
                ButtonAlphaChange(fragmentButtons[1], 1f);
                break;
            case "ExtraFragment":
                ButtonAlphaChange(fragmentButtons[2], 1f);
                break;
        }

        target = targetFragment;
        targetFragment.transform.SetParent(nextFragment.transform);
        targetFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        fragmentChangeAnim.Play("FragmentChangeAnim");
        Invoke("OnEndChangeAnimation", 1f);
    }

    private void OnEndChangeAnimation()
    {
        // set default parent to converted fragments(prevent unwanted movemetns of the converted fragments while initiallization)
        target.transform.SetParent(mainUI.transform);
        currentFragment.transform.GetChild(0).transform.SetParent(mainUI.transform);

        // initiallize position to animation start pos
        currentFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        nextFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(1440, 0);

        target.transform.SetParent(currentFragment.transform);
        target.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        isConvertingUi = false;
    }

    /// <summary>
    /// OnClick Parts Upgrade button
    /// </summary>
    /// <param name="name"></param>
    public void OpenPartsUpgradeBase(string name)
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        currentParts = name;
        partsUpgradeBase.SetActive(true);
        selectedPartsIdx = selectedParts[currentParts];
        switch (selectedPartsIdx)
        {
            case 0:
                partsUpgradeInfo = LocalDatabaseManager.instance.PartsMissile;
                break;
            case 1:
                partsUpgradeInfo = LocalDatabaseManager.instance.PartsLaser;
                break;
            case 2:
                partsUpgradeInfo = LocalDatabaseManager.instance.PartsBarrier;
                break;
            case 3:
                partsUpgradeInfo = LocalDatabaseManager.instance.PartsEmp;
                break;
            default:
                Debug.Log("selected part is out of index range");
                break;
        }

        // Set each UI Component with Selected Parts
        selectedPartsName.text = currentParts;
        selectedPartsDescription.text = partsDescriptions[selectedPartsIdx];
        selectedPartsImage.transform.GetChild(selectedPartsIdx).gameObject.SetActive(true);
        ability1Title.text = partsInfo[selectedPartsIdx, 0, 0];
        ability2Title.text = partsInfo[selectedPartsIdx, 1, 0];
        ability3Title.text = partsInfo[selectedPartsIdx, 2, 0];
        
        SetPartsUpgradeJemText();
    }

    public void ClosePartsUpgradeBase()
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        selectedPartsImage.transform.GetChild(selectedPartsIdx).gameObject.SetActive(false);
        partsUpgradeBase.SetActive(false);
    }

    /// <summary>
    /// On Parts Upgrade Button Clicked
    /// </summary>
    /// <param name="index">param indicated 'which' button clicked, the order matters</param>
    public void OnUpgradeButtonClicked(int index)
    {
        // return if current value is already max
        if (partsUpgradeInfo[index] >= LocalDatabaseManager.instance.MaxUpgradeInfo[index])
            return;
        // return if current jem is insufficient
        if (LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, index, partsUpgradeInfo[index]] > LocalDatabaseManager.instance.JemCount)
            return;

        SoundManager.instance.PlaySFX("PartsUpgradeSound");

        // set Local data & UI components
        LocalDatabaseManager.instance.JemCount -= LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, index, partsUpgradeInfo[index]];
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString() + " J";
        partsUpgradeInfo[index] += 1;
        SetPartsUpgradeJemText();
        LocalDatabaseManager.instance.SavePartsData();
        LocalDatabaseManager.instance.SaveGameData();
    }

    private void SetPartsUpgradeJemText()
    {
        ability1Description.text = partsInfo[selectedPartsIdx, 0, 1] + LocalDatabaseManager.instance.PartsStatInfo[currentParts][0, partsUpgradeInfo[0]].ToString();
        ability2Description.text = partsInfo[selectedPartsIdx, 1, 1] + LocalDatabaseManager.instance.PartsStatInfo[currentParts][1, partsUpgradeInfo[1]].ToString();
        ability3Description.text = partsInfo[selectedPartsIdx, 2, 1];


        if (partsUpgradeInfo[0] >= LocalDatabaseManager.instance.MaxUpgradeInfo[0])
            ability1UpgradeJem.text = "Max";
        else
            ability1UpgradeJem.text = LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, 0, partsUpgradeInfo[0]].ToString() + " J";

        if (partsUpgradeInfo[1] >= LocalDatabaseManager.instance.MaxUpgradeInfo[1])
            ability2UpgradeJem.text = "Max";
        else
            ability2UpgradeJem.text = LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, 1, partsUpgradeInfo[1]].ToString() + " J";

        if (partsUpgradeInfo[2] >= LocalDatabaseManager.instance.MaxUpgradeInfo[2])
            ability3UpgradeJem.text = "Max";
        else
            ability3UpgradeJem.text = LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, 2, partsUpgradeInfo[2]].ToString() + " J";
    }

    // func for testing
    public void ResetPartsInfo()
    {
        LocalDatabaseManager.instance.PartsMissile = new int[3] { 0, 0, 0 };
        LocalDatabaseManager.instance.PartsLaser = new int[3] { 0, 0, 0 };
        LocalDatabaseManager.instance.PartsBarrier = new int[3] { 0, 0, 0 };
        LocalDatabaseManager.instance.PartsEmp = new int[3] { 0, 0, 0 };
        LocalDatabaseManager.instance.SavePartsData();
    }

    public void SetJem()
    {
        LocalDatabaseManager.instance.JemCount = int.Parse( input.text);
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString() + " J";
        LocalDatabaseManager.instance.SaveGameData();
    }
}
