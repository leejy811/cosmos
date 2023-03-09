using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUiManager : MonoBehaviour
{
    #region UI Components
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject currentFragment;
    [SerializeField] private GameObject nextFragment;
    [SerializeField] private Animation fragmentChangeAnim;
    #endregion

    #region Member Variables
    private GameObject target;
    private bool isConvertingFragment = false;
    #endregion

    public void ChangeScene()
    {
        SceneManager.LoadScene("InGameScene");  
    }

    public void OnClickFragmentChange(GameObject targetFragment)
    {
        if (currentFragment.transform.GetChild(0) == targetFragment.transform || isConvertingFragment)
            return;
        isConvertingFragment = true;
        target = targetFragment;
        targetFragment.transform.parent = nextFragment.transform;
        targetFragment.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        fragmentChangeAnim.Play("FragmentChangeAnim");
        Invoke("OnEndChangeAnimation", 1f);
    }

    public void OnEndChangeAnimation()
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
