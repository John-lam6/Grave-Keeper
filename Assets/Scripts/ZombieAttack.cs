using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    public GameObject player;
    public float attackDelay = 0.11f;
    public float attackCooldown = 1.5f;
    public float attackRange;
    private bool isAttacking = false;
    public ZombieController zombieController;
    public int damage = 1;
    private float lastAttackTime = -999f;
    private bool canMove = true;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        zombieController = GetComponent<ZombieController>();
        player = GameObject.FindWithTag("Target");
    }

    // Update is called once per frame
    void Update() {
        if (zombieController.isDead()) return;
        
        Vector3 toTarget = player.transform.position - transform.position;
        toTarget.y = 0;
        float targetDistance = toTarget.magnitude;
        toTarget.Normalize();
        if (canMove && !zombieController.isDead() && targetDistance <= attackRange && isAttacking == false && Time.time >= lastAttackTime + attackCooldown) {
            StartCoroutine(Attack());
        }
    }

    public IEnumerator Attack() {
        isAttacking = true;
        lastAttackTime = Time.time;
        m_Animator.SetBool("isAttacking", isAttacking);
        yield return new WaitForSeconds(attackDelay);

        Vector3 toTarget = player.transform.position - transform.position;
        toTarget.y = 0;
        float targetDistance = toTarget.magnitude;
        toTarget.Normalize();

        if (!zombieController.isDead() && targetDistance <= attackRange) {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            health.StartCoroutine(health.Damage(damage));
        }
        
        yield return new WaitForSeconds(attackCooldown - attackDelay);
        
        isAttacking = false;
        m_Animator.SetBool("isAttacking", isAttacking);
    }

    public void setCanMove(bool canmove) {
        canMove = canmove;
    }
}
