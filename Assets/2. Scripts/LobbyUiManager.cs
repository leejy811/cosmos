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
    #endregion

    #region Member Variables
    private GameObject target;      // reference to the selected fragment
    private GameObject currentPart; // reference to the currently equiped part obj
    private bool isConvertingUi = false;
    private float currentTime = 0f;  
    private float fadeoutTime = 1f;
    private int selectedPartsIdx = -1;
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

    #endregion

    private void Start()
    {
        SetUi();
        SoundManager.instance.PlayBGM("LobbyBGM");
    }

    // Set user data to the Text at the start of the game
    private void SetUi()
    {
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString()+" J";
        highScore.text = "High Score : "+LocalDatabaseManager.instance.HighScore.ToString();
        currentPart= GameObject.Find("Parts" + LocalDatabaseManager.instance.CurrentParts);
        if (currentPart != null)
            currentPart.GetComponent<Animation>().Play("PartsUiEquipAnim");
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

    public void OpenPartsUpgradeBase(string name)
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        partsUpgradeBase.SetActive(true);
        selectedPartsIdx = selectedParts[name];

        selectedPartsName.text = name;
        selectedPartsDescription.text = partsDescriptions[selectedPartsIdx];
        selectedPartsImage.transform.GetChild(selectedPartsIdx).gameObject.SetActive(true);

    }

    public void ClosePartsUpgradeBase()
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        selectedPartsImage.transform.GetChild(selectedPartsIdx).gameObject.SetActive(false);
        partsUpgradeBase.SetActive(false);
    }
}
