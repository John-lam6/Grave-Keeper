using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public GameManager m_GameManager;
    public Button m_StartButton;      
    public StartMenuSlot[] m_PlayerSlots; 
    public TMP_Text m_StartButtonText; 
    private int selectedSlotIndex = -1;
    private LanguageManager languageManager;
    
    // Start is called before the first frame update
    void Start()
    {
        languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager == null) Debug.Log("NULL");
        m_StartButton.onClick.AddListener(StartGame);
        m_StartButton.interactable = false;
        m_StartButtonText = m_StartButton.GetComponentInChildren<TMP_Text>();
        m_StartButtonText.text = languageManager.currentLanguage.GetValue("Select_Gun");
        
        var gunsPrefabs =
            new[]
            {
                m_GameManager.gunPrefab1, m_GameManager.gunPrefab2, m_GameManager.gunPrefab3
            };
        
        for (int i = 0; i < 3; ++i) {
            var slot = m_PlayerSlots[i];
            slot.SetGunPreview(gunsPrefabs.Length > i ? gunsPrefabs[i] : gunsPrefabs[0]);
            int slotIndex = i;
            slot.selectButton.onClick.AddListener(() =>
            {
                SelectGun (slotIndex);
            });
        }
    }

    void SelectGun(int slotIndex) {
        if (selectedSlotIndex != -1 && selectedSlotIndex != slotIndex) {
            m_PlayerSlots[selectedSlotIndex].DeselectGun();
        }
        
        selectedSlotIndex = slotIndex;
        m_PlayerSlots[slotIndex].SelectGun();
        
        m_StartButtonText.text = languageManager.currentLanguage.GetValue("Start");
        m_StartButton.interactable = true;
    }

    void StartGame() {
        m_StartButton.gameObject.SetActive(false);
        if (selectedSlotIndex == -1) return;
        
        GameObject selectedGunPrefab = m_PlayerSlots[selectedSlotIndex].GunPrefab;
        m_GameManager.StartGameWithGun(selectedGunPrefab);
        

        foreach (var slot in m_PlayerSlots) {
            if (slot.GunPreview != null) {
                Destroy(slot.GunPreview);
            }
        }
    }

    public void ShowWeaponSelection() {
        if (languageManager == null) languageManager = FindObjectOfType<LanguageManager>();
        //Debug.Log("Hi");
        // Reset selection
        selectedSlotIndex = -1;
        m_StartButton.interactable = false;
        m_StartButtonText.text = languageManager.currentLanguage.GetValue("Select_Gun");
    
        // Reset all slots to deselected state
        foreach (var slot in m_PlayerSlots) {
            slot.DeselectGun();
        }
    }
}
