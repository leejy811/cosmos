using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsElement : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    public bool IsUnlocked { get; private set; } = false;

    public void PartsUnlock()
    {
        unlockPanel.SetActive(false);
        IsUnlocked = true;
    }
}
