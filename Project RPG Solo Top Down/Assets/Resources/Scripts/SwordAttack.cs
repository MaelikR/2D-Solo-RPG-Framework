using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider;
    public float damage = 3;
    public float attackCooldown = 1.0f;
    private float lastAttackTime = 0;
    private bool isAttacking = false;
    Vector2 attackOffset;

    private void Start()
    {
        attackOffset = transform.localPosition;
        swordCollider.isTrigger = true; // Assurez-vous que le collider est un trigger
    }
    public void StopAttack()
    {
        swordCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision Detected with: " + other.gameObject.name);
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        // Ajout de la logique pour infliger des dégâts au joueur
        else if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
    public void Attack(bool isFacingRight)
    {
        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;

        isAttacking = true;
        swordCollider.enabled = true;
        transform.localPosition = isFacingRight ? attackOffset : new Vector3(-attackOffset.x, attackOffset.y);

        StartCoroutine(DisableColliderAfterDelay(0.3f)); // Ajuster la durée de l'attaque

        lastAttackTime = Time.time;
    }

    private IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        swordCollider.enabled = false;
        isAttacking = false;
    }
}

public interface IDamageable
{
    void TakeDamage(float damage);
}
