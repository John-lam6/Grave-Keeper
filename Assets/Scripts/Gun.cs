using UnityEngine;

public class Gun : MonoBehaviour {
    public GunSO gunso;
    private float lastShotTime;
    public int currentAmmo;
    public Transform firePoint;
    public Transform playerTransform;
    
    public void Start() {
        currentAmmo = gunso.maxAmmo;
    }
    
    // Only shoot when cooldown between shots is long enough
    public void TryShoot() {
        if (Time.time > lastShotTime + gunso.shotCooldown) {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void Shoot() {
        currentAmmo--;
        
        for (int i = 0; i < gunso.bulletsPerShot; i++) {
            
            Vector3 direction = playerTransform.forward;
            if (gunso.spreadAngle > 0f) {
                float angle = Random.Range (-gunso.spreadAngle / 2f, gunso.spreadAngle / 2f);
                direction = Quaternion.Euler(0f, angle, 0f) * direction;
            }
            Bullet bullet = Instantiate(gunso.bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            bullet.bulletData = gunso.bulletData;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = direction * gunso.bulletData.speed;
        }
        
    }
}
