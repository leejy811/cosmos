using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

/// <summary>
/// Basic Button Behavior Effect Controller. 
/// Gives Punch, Scaling, and Prevent multi-Clicking
/// </summary>
public class ButtonBehavior : MonoBehaviour
{
    private Button myButton;
    private Vector3 originalScale;
    private bool isTweening;

    void Start()
    {
        isTweening = false;
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnButtonClick);    // Set Callback Method
        originalScale = transform.localScale;
    }

    // Give Punch effect when the button clicked
    private void OnButtonClick()
    {
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            transform.DOPunchScale(originalScale * 0.2f, 0.2f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
    }
}
