using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerShoot : MonoBehaviour {
    public Gun m_Gun;
    public TMP_Text ammoText;
    public Slider reloadSlider;
    public Color damageColor;

    private bool reloading = false;
    private GunSO gunSO;
    private bool canMove = false;
    
    private void Start() {
        gunSO = m_Gun.gunso;
        m_Gun.currentAmmo = gunSO.maxAmmo;
    }
    
    void Update() {
        if (canMove && Input.GetButton("Fire1") && m_Gun.currentAmmo >= 1 && !reloading) {
            m_Gun.TryShoot();
        }
        else if (canMove && m_Gun.currentAmmo <= 0 && !reloading) {
            StartCoroutine(ReloadGun(m_Gun, gunSO));
        }
        else if (canMove && Input.GetKeyDown(KeyCode.R) && !reloading && m_Gun.currentAmmo < gunSO.maxAmmo) {
            StartCoroutine(ReloadGun(m_Gun, gunSO));
        }
        
        ammoText.text = m_Gun.currentAmmo + " / " + gunSO.maxAmmo;
    }

    private IEnumerator ReloadGun(Gun gun, GunSO gunso) {
        reloading = true;
        reloadSlider.gameObject.SetActive(true);
        reloadSlider.value = 0;
        reloadSlider.DOValue(1, gunso.reloadTime).SetEase(Ease.Linear);

        yield return new WaitForSeconds(gunso.reloadTime);

        reloadSlider.gameObject.SetActive(false);
        gun.currentAmmo = gunso.maxAmmo;
        reloading = false;
    }

    public void updateGun(Gun prefab) {
        m_Gun = prefab;
        gunSO = prefab.gunso;
        m_Gun.currentAmmo = gunSO.maxAmmo;
    }

    public void setCanMove (bool canMove) {
        this.canMove = canMove;
    }
}
