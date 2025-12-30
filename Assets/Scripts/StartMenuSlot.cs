using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class StartMenuSlot : MonoBehaviour {
    public RectTransform previewPosition; // The Transform on which to place the Tank preview so it display at the right place on screen
    public TextMeshProUGUI gunStats; // The Text to use to display the tank stats
    public Button selectButton;

    public Image backgroundImage; // Assign in inspector if you want visual feedback
    public Color selectedColor = Color.green;
    public Color deselectedColor = Color.white;
    public GameObject GunPreview { get; set; } // The preview instance that show this gun rotating in the menu
    public GameObject GunPrefab { get; private set; } // The prefab this slot is based on
    public Camera m_MenuCamera;  
    private bool isSelected = false;

    public GunSO gunstats;

    private LanguageManager languageManager;

    // Start is called before the first frame update
    void Start() {
        languageManager = FindObjectOfType<LanguageManager>();
        
        //selectButton.onClick.AddListener(Clicked);
        //selectButton.interactable = true;
        m_MenuCamera = GetComponentInParent<Camera>();

        if (backgroundImage != null) {
            backgroundImage.color = deselectedColor;
        }
    }

    // Update is called once per frame
    void Update() {
        if (GunPreview != null) {
            GunPreview.transform.Rotate(Vector3.up, 45.0f * Time.deltaTime);
        }
    }

    public void Clicked() {
        //Debug.Log("Clicked");
        if (isSelected) DeselectGun();
        else SelectGun();
    }
    
    public void SelectGun() {
        isSelected = true;
        //selectButton.interactable = true;

        if (backgroundImage != null) {
            //Debug.Log("color change1");
            backgroundImage.color = selectedColor;
        }
    }
    
    public void DeselectGun() {
        isSelected = false;
        //selectButton.interactable = false;

        if (backgroundImage != null) {
            //Debug.Log("color change2");
            backgroundImage.color = deselectedColor;
        }
    }
    
    public void SetGunPreview(GameObject prefab) {
        if (languageManager == null) languageManager = FindObjectOfType<LanguageManager>();
        
        if (GunPreview != null) {
            Destroy(GunPreview);
        }
        GunPrefab = prefab;
        GunPreview = Instantiate(prefab);
        
        gunStats.text = $"{languageManager.currentLanguage.GetValue("Attack_Speed")}: {gunstats.shotCooldown}\n{languageManager.currentLanguage.GetValue("Damage")}: {gunstats.bulletData.damage}\n{languageManager.currentLanguage.GetValue("Bullets_P_Shot")}: {gunstats.bulletsPerShot}\n{languageManager.currentLanguage.GetValue("Reload_Time")}: {gunstats.reloadTime}\n{languageManager.currentLanguage.GetValue("Max_Ammo")}: {gunstats.maxAmmo}\n{languageManager.currentLanguage.GetValue("Can_Pierce")}: {gunstats.bulletData.canPierce}";

        //move it to the right preview position so it appears in the right spot on screen
        var position = m_MenuCamera.WorldToScreenPoint(previewPosition.position);
        GunPreview.transform.position = m_MenuCamera.ScreenToWorldPoint(position) + Vector3.back * 3.0f;

        //Disable all audio
        // audioSource = TankPreview.GetComponentsInChildren<AudioSource>();
        //foreach (var source in audioSource) {
        //    Destroy(source);
        //}
    }

}
