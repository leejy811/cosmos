using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    #region UI Components
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject currentFragment;
    [SerializeField] private GameObject nextFragment;
    [SerializeField] private Animation fragmentChangeAnim;
    [SerializeField] private Image panel;
    [SerializeField] private Image[] fragmentButtons;
    [SerializeField] private Text jemCount;
    [SerializeField] private Text highScore;
    #endregion

    #region Member Variables
    private GameObject target;
    private bool isConvertingFragment = false;
    private float currentTime = 0f;  
    private float fadeoutTime = 1f;
    #endregion

    private void Start()
    {
        SetUi();
    }

    private void SetUi()
    {
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString()+" J";
        highScore.text = "High Score : "+LocalDatabaseManager.instance.HighScore.ToString();
    }

    public void ChangeScene()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        while (alpha.a < 1)
        {
            currentTime += Time.deltaTime / fadeoutTime;
            alpha.a = Mathf.Lerp(0, 1, currentTime);
            panel.color = alpha;
            yield return null;
        }
        SceneManager.LoadScene("InGameScene");
    }

    private void ButtonAlphaChange(Image image, float a)
    {
        Color alpha = image.color;
        alpha.a = a;
        image.color = alpha;
    }

    public void OnClickFragmentChange(GameObject targetFragment)
    {
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

        if (currentFragment.transform.GetChild(0) == targetFragment.transform || isConvertingFragment)
            return;
        isConvertingFragment = true;
        target = targetFragment;
        targetFragment.transform.parent = nextFragment.transform;
        targetFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        fragmentChangeAnim.Play("FragmentChangeAnim");
        Invoke("OnEndChangeAnimation", 1f);
    }

    private void OnEndChangeAnimation()
    {
        target.transform.parent = mainUI.transform;
        currentFragment.transform.GetChild(0).transform.parent = mainUI.transform;

        currentFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        nextFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(1440, 0);

        target.transform.parent = currentFragment.transform;
        target.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        isConvertingFragment = false;
    }
}
