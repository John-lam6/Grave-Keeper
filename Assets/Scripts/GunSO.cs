using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)] 
public class GunSO : ScriptableObject {
    public string gunName;
    public float shotCooldown;
    public BulletSO bulletData;
    public Bullet bulletPrefab;
    public int bulletsPerShot = 1;
    public float spreadAngle = 0f;
    public float reloadTime;
    public int maxAmmo = 5;
}
