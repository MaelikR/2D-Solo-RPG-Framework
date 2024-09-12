using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float attackRange = 1.0f;
    public float attackCooldown = 2.0f;
    public float sightRange = 3f;
    private Transform player;
    private bool isAttacking = false;
    public float health = 55f;
    private Animator animator;
    private bool isDefeated = false;
    private bool isPatrolling = false;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    public Slider healthSlider;
    private Rigidbody2D rb;

    private void Start()
    {
        // Obtenez d'abord la référence du joueur
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        // Initialisez les autres références
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(EnemyBehaviour());
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (!isAttacking && !isPatrolling)
            {
                isPatrolling = true;
                StartCoroutine(MoveRandomDirection());
            }

            yield return null;
        }
    }

    private IEnumerator MoveRandomDirection()
    {
        while (true)
        {
            // Générer une direction aléatoire
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;

            // Déplacer vers la direction aléatoire
            MoveTowards(randomDirection);

            // Attendre pendant une courte période
            yield return new WaitForSeconds(UnityEngine.Random.Range(2.5f, 5.5f)); // Ajustez la plage selon vos besoins
        }
    }

    private IEnumerator EnemyBehaviour()
    {
        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= sightRange)
            {
                isPatrolling = false; // Arrêter la patrouille lorsqu'il voit le joueur
                StartCoroutine(ChasePlayer());
            }

            yield return null;
        }
    }

    private IEnumerator ChasePlayer()
    {
        while (true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            MoveTowards(direction);

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                Attack(direction);
                yield break;
            }

            yield return new WaitForSeconds(0.1f); // Ajoutez une petite pause pour éviter un comportement saccadé
        }
    }

    public void Attack(Vector2 attackDirection)
    {
        isAttacking = true;
        if (playerController != null)
        {
            // Inflige des dégâts au joueur via le PlayerController
            playerController.TakeDamage(5f); // Ajustez la valeur des dégâts selon vos besoins
        }
        // Jouer l'animation d'attaque
        animator.SetTrigger("Attack");

        // Réinitialiser l'attaque après un délai
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void MoveTowards(Vector2 direction)
    {
        Vector2 targetPosition = (Vector2)transform.position + (direction * moveSpeed * Time.deltaTime);
        rb.MovePosition(targetPosition); // Utilisez 'rb' ici pour le mouvement

        // Changer la direction de l'ennemi en fonction du mouvement
        if (direction.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void Update()
    {
        // Mise à jour régulière de la barre de vie
        UpdateHealthSlider();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Death();
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            // Calculer la valeur normalisée pour la barre de vie
            float normalizedHealth = health / 55f; // Ajustez selon la santé maximale

            // Mettre à jour la valeur de la barre de vie
            healthSlider.value = normalizedHealth;
        }
    }

    private void Death()
    {
        if (!isDefeated)
        {
            isDefeated = true;
            animator.SetTrigger("Defeated");

            // Lancer la coroutine pour retarder la destruction
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        // Attendre une courte durée pour permettre à l'animation "Defeated" de se jouer
        yield return new WaitForSeconds(0.89f); // Ajustez la durée selon vos besoins

        // Désactiver/détruire l'objet
        Destroy(gameObject);
    }
}
