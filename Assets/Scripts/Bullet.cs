using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    public BulletSO bulletData;
    public Rigidbody m_Rigidbody;
    
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        
        // makes sure it doesn't slow down or change y-level
        m_Rigidbody.useGravity = false;
        m_Rigidbody.drag = 0f;
        
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.freezeRotation = true;
        //m_Rigidbody.velocity = transform.root.forward * bulletData.speed;
    }
    
    void FixedUpdate() {
        //float speed = bulletData.speed * Time.deltaTime;
        m_Rigidbody.velocity = transform.forward * bulletData.speed;
        
        // if max distance travelled, destroy
        if (Vector3.Distance(startPosition, transform.position) >= bulletData.maxDistance) {
            Destroy(gameObject);
        }
    }
    
    
    private void OnCollisionEnter(Collision collision) {
        // if bullet hits an obstacle in the environment, destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default")) {
            Destroy(gameObject);
            return;
        }
    }
    

    private void OnTriggerEnter(Collider other) {
        ZombieController hitEnemy = other.gameObject.GetComponent<ZombieController>();
        
        if (hitEnemy != null && !hitEnemy.isDead()) {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                
            hitEnemy.rb.AddForce(knockbackDirection * bulletData.knockbackForce, ForceMode.Impulse);
            
            hitEnemy.StartCoroutine(hitEnemy.DamageAgent(bulletData.damage));

            if (!bulletData.canPierce) Destroy(gameObject);
        }
    }
}
