using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyUiManager : MonoBehaviour
{
    // Manage all UI Components in the Lobby Scene
    // Activate UI Animations and set UI informations containing user data

    #region UI Components
    //main ui
    [SerializeField] private RectTransform mainUiContainer;
    [SerializeField] private GameObject partsUpgradeBase;
    [SerializeField] private Image panel;               // For Fade-out effect
    [SerializeField] private Image[] fragmentButtons;   // reference to 3 buttons at the bottom
    [SerializeField] private Text jemCount;
    [SerializeField] private Text highScore;
    [SerializeField] private Text ticket;
    [SerializeField] private Text equipment;
    [SerializeField] private Image jemIcon;
    [SerializeField] private ParticleSystem uiParticle;

    //parts ui
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
    [SerializeField] private Image[] partsUpgradeButtons;

    //settings ui
    [SerializeField] private Scrollbar bgmSlider;
    [SerializeField] private Scrollbar sfxSlider;
    [SerializeField] private SwitchButton bloomToggle;
    [SerializeField] private SwitchButton hitToggle;

    // Popup
    [SerializeField] private GameObject exitPopup;
    [SerializeField] private GameObject battlePopup;
    [SerializeField] private Text ticketNum;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject popupPanel;
    #endregion

    #region Member Variables
    private Vector3 originalJemScale;
    private bool isTweening=false;
    private GameObject target;      // reference to the selected fragment
    private GameObject currentPart; // reference to the currently equiped part obj
    private bool isConvertingUi = false;
    private int currentFragment = 0;
    private float currentTime = 0f;  
    private float fadeoutTime = 1f;
    private int selectedPartsIdx = 0;
    private string currentParts;
    private int[] partsUpgradeInfo;
    private float punchScale = 0.2f;
    public bool isPopupOpen { get; private set; } = false;
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
        {{"Parts Damage","Missile Damage +"},{ "Parts Speed","Missile Speed +"},{ "Abilities","Increase Missile Range"} },
        {{"Parts Damage","Laser Damage +"},{ "Parts Speed","Laser Speed +"},{ "Abilities","Make Another Laser"} },
        {{"Parts Damage","Barrier Damage +"},{ "Parts Speed","Speed Reduction +"},{ "Abilities","Make Shield Initially"} },
        {{"Parts Damage","Emp Damage +"},{ "Parts Speed","Cool-Time -"},{ "Abilities","Additional Knock-Back Effect"} }
    };

    [SerializeField] AdsManager adsManager;
    #endregion

    #region For Debug
    [SerializeField] private InputField input;
    #endregion

    private void Start()
    {
        SetUi();
    }

    // Set user data to the Text at the start of the game
    private void SetUi()
    {
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString()+" J";
        highScore.text =LocalDatabaseManager.instance.HighScore;
        ticket.text = LocalDatabaseManager.instance.Ticket.ToString();
        equipment.text = LocalDatabaseManager.instance.CurrentParts;
        currentPart= GameObject.Find("Parts" + LocalDatabaseManager.instance.CurrentParts);
        if (currentPart != null)
            currentPart.GetComponent<Animation>().Play("PartsUiEquipAnim");
        originalJemScale = jemIcon.transform.localScale;

        bgmSlider.value = SoundManager.instance.GetBgmVolume();
        sfxSlider.value = SoundManager.instance.GetSfxVolume();
        bgmSlider.onValueChanged.AddListener(SetBgmSlider);
        sfxSlider.onValueChanged.AddListener(SetSfxSlider);

        bloomToggle.SetSwitch(GameManger.instance.onBloomEffect);
        hitToggle.SetSwitch(GameManger.instance.onHitEffect);
    }


    /// <summary>
    /// Make Fade-out effect when the 'Battle' button clicked
    /// </summary>
    public void ChangeScene(bool useTicket)
    {
        LocalDatabaseManager.instance.isTicketMode = useTicket;
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
        GameManger.instance.StartGame();
    }

    /// <summary>
    /// Converting Parts in Lobby Scene, Parts Fragment
    /// </summary>
    /// <param name="part"></param>
    public void EquipParts(GameObject part)
    {
        // Conduct converting if not exceptional situation
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
        equipment.text = LocalDatabaseManager.instance.CurrentParts;
        switch (LocalDatabaseManager.instance.CurrentParts)
        {
            case "Missile":
                LocalDatabaseManager.instance.PartsValue = LocalDatabaseManager.instance.PartsMissile;
                break;
            case "Laser":
                LocalDatabaseManager.instance.PartsValue = LocalDatabaseManager.instance.PartsLaser;
                break;
            case "Barrier":
                LocalDatabaseManager.instance.PartsValue = LocalDatabaseManager.instance.PartsBarrier;
                break;
            case "Emp":
                LocalDatabaseManager.instance.PartsValue = LocalDatabaseManager.instance.PartsEmp;
                break;
        }
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
    public void OnClickFragmentChange(int targetFragment)
    {
        if (currentFragment == targetFragment || isConvertingUi)
            return;

        isConvertingUi = true;
        SoundManager.instance.PlaySFX("BasicButtonSound");

        // Set the alpha value of each fragment button(if selected, assign 1)
        foreach (Image i in fragmentButtons)
            ButtonAlphaChange(i, 0.15f);
        switch (targetFragment)
        {
            case 0:
                ButtonAlphaChange(fragmentButtons[0], 1f);
                break;
            case 1:
                ButtonAlphaChange(fragmentButtons[1], 1f);
                break;
            case 2:
                ButtonAlphaChange(fragmentButtons[2], 1f);
                break;
        }

        float moveAmount = (currentFragment - targetFragment) * 1440;
        currentFragment = targetFragment;
        StartCoroutine(MoveFragment(moveAmount));
    }

    // use Coroutine + DOTween
    IEnumerator MoveFragment(float distance)
    {
        var tween = mainUiContainer.DOLocalMoveX(mainUiContainer.localPosition.x+ distance, 1f);
        yield return tween.WaitForCompletion();
        isConvertingUi = false; //enable fragment change after converting finished
    }


    /// <summary>
    /// OnClick Parts Upgrade button
    /// </summary>
    /// <param name="name"></param>
    public void OpenPartsUpgradeBase(string name)
    {
        isPopupOpen = true;
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

    public void OpenExitPopup()
    {
        isPopupOpen = true;
        exitPopup.SetActive(true);
    }

    public void CloseExitPopup()
    {
        if (!exitPopup.activeSelf)
            return;
        isPopupOpen = false;
        exitPopup.SetActive(false);
    }

    public void ClosePartsUpgradeBase()
    {
        if (!partsUpgradeBase.activeSelf)
            return;
        isPopupOpen = false;
        SoundManager.instance.PlaySFX("BasicButtonSound");
        selectedPartsImage.transform.GetChild(selectedPartsIdx).gameObject.SetActive(false);
        partsUpgradeBase.SetActive(false);
    }

    public void OpenBattlePopup()
    {
        isPopupOpen = true;
        battlePopup.SetActive(true);
        ticketNum.text = LocalDatabaseManager.instance.Ticket + " / 10";
    }

    public void CloseBattlePopup()
    {
        if (!battlePopup.activeSelf)
            return;
        isPopupOpen = false;
        battlePopup.SetActive(false);
    }

    public void OpenSettingsPopup()
    {
        isPopupOpen = true;
        settingsPopup.SetActive(true);
        popupPanel.transform.DOLocalMoveY(1000f, 0.2f).SetEase(Ease.InBack).SetRelative(true);
    }

    public void CloseSettingsPopup()
    {
        if (!settingsPopup.activeSelf)
            return;
        isPopupOpen = false;
        popupPanel.transform.DOLocalMoveY(-1000f, 0.2f).SetEase(Ease.InBack).OnComplete(() => settingsPopup.SetActive(false));
    }

    public void ClosePopupUi()
    {
        ClosePartsUpgradeBase();
        CloseExitPopup();
        CloseBattlePopup();
        CloseSettingsPopup();
    }

    /// <summary>
    /// On Parts Upgrade Button Clicked
    /// </summary>
    /// <param name="index">param indicated 'which' button clicked, the order matters</param>
    public void OnUpgradeButtonClicked(int index)
    {
        bool returnFlag = false;
        Vector3 originalScale= partsUpgradeButtons[index].transform.localScale;

        // return if current value is already max
        if (partsUpgradeInfo[index] >= LocalDatabaseManager.instance.MaxUpgradeInfo[index])
            returnFlag=true;
        // return if current jem is insufficient
        else if (LocalDatabaseManager.instance.PartsUpgradeJem[selectedPartsIdx, index, partsUpgradeInfo[index]] > LocalDatabaseManager.instance.JemCount)
            returnFlag = true;

        if (returnFlag)
        {
            SoundManager.instance.PlaySFX("ButtonDenied");
            if (!isTweening)    // Prevent multi-clicking
            {
                isTweening = true;
                partsUpgradeButtons[index].transform.DOPunchPosition(new Vector3(20, 0, 0), 0.5f, 10, 1f).OnComplete(() =>
                {
                    isTweening = false;
                });
            }
            return;
        }

        SoundManager.instance.PlaySFX("PartsUpgradeSound");
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            partsUpgradeButtons[index].transform.DOPunchScale(originalScale * punchScale, 0.2f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }

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
        int description1 = (int)(LocalDatabaseManager.instance.PartsStatInfo[currentParts][0, partsUpgradeInfo[0]] * 100);
        int description2 = (int)(LocalDatabaseManager.instance.PartsStatInfo[currentParts][1, partsUpgradeInfo[1]] * 100);
        ability1Description.text = partsInfo[selectedPartsIdx, 0, 1] + description1.ToString();
        ability2Description.text = partsInfo[selectedPartsIdx, 1, 1] + description2.ToString();
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

    private void SetBgmSlider(float value)
    {
        SoundManager.instance.SetBgmVolume(value);
    }

    private void SetSfxSlider(float value)
    {
        SoundManager.instance.SetSfxVolume(value);
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

        // Stop the particle system
        uiParticle.Stop();
        // Clear any existing particles
        uiParticle.Clear();
        // Start the particle system
        uiParticle.Play();
    }

    public void JemParticleEffect()
    {
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            jemIcon.transform.DOPunchScale(originalJemScale * 0.5f, 0.1f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
    }

    public void OnAdsButtonClick()
    {
        adsManager.ShowRewardedAd();
    }

    public void EndAdsReward()
    {
        ChangeScene(true);
    }

    public void OnClickUseTicketButton()
    {
        if (LocalDatabaseManager.instance.Ticket < 10)
            return;
        LocalDatabaseManager.instance.Ticket -= 10;
        LocalDatabaseManager.instance.SaveGameData();
        ChangeScene(true);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
