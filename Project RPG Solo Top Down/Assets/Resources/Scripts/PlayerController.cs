using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public bool isFacingRight = true;
    public Camera playerCamera;
    private AudioListener audioListener;

    [SerializeField] private TMP_Text usernameText;

    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    public float maxHealth = 100f;
    public float currentHealth;
    public float enduranceRegenRate = 5f;
    public float dashSpeedMultiplier = 2f;
    public float maxMana = 100f;
    public float currentMana;
    public float dashEnduranceCost = 20f;
    public float dashDistance = 5f;
    public float maxEndurance = 100f;
    public float currentEndurance;
    public float dashDuration = 0.5f;
    private bool isDashing = false;

    public Slider healthSlider;
    public Slider manaSlider;
    public Slider enduranceSlider;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    readonly List<RaycastHit2D> castCollisions = new();

    bool canMove = true;
    public string KnightHome;
    public new Camera camera;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentEndurance = maxEndurance;

        UpdateHealthBar();
        UpdateManaBar();
        UpdateEnduranceBar();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioListener = GetComponent<AudioListener>();

        RegenerateEndurance();
    }

    void OnDash()
    {
        animator.SetTrigger("DashTrigger");
        StartCoroutine(DashFunction());
    }

    IEnumerator DashFunction()
    {
        UseEndurance(dashEnduranceCost);

        isDashing = true;

        Vector2 initialPosition = rb.position;
        Vector2 dashDestination = initialPosition + movementInput.normalized * dashDistance;

        UpdateEnduranceBar();

        float elapsed = 0f;
        LayerMask dashLayerMask = LayerMask.GetMask("Default");
        while (elapsed < dashDuration)
        {
            Vector2 projectedPosition = Vector2.Lerp(initialPosition, dashDestination, elapsed / dashDuration);
            RaycastHit2D hit = Physics2D.Raycast(rb.position, (projectedPosition - rb.position).normalized, Vector2.Distance(rb.position, projectedPosition), dashLayerMask);

            if (hit.collider == null)
            {
                rb.MovePosition(projectedPosition);
                rb.velocity = dashSpeedMultiplier * moveSpeed * rb.velocity.normalized;
            }
            else
            {
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
        }
    }

    void Update()
    {
        RegenerateEndurance();
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Vector3 teleportPosition = new Vector3(0, 0, 0);
        transform.position = teleportPosition;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void MovePlayer()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
            }

            if (!success)
            {
                success = TryMove(new Vector2(0, movementInput.y));
            }

            animator.SetBool("isMoving", success);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            Vector2 newPosition = rb.position + moveSpeed * Time.fixedDeltaTime * direction;
            int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                transform.position = newPosition;
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
        SwordAttack();
    }

    void RegenerateEndurance()
    {
        if (!isDashing)
        {
            currentEndurance += enduranceRegenRate * Time.deltaTime;
            currentEndurance = Mathf.Min(currentEndurance, maxEndurance);
            UpdateEnduranceBar();
        }
    }

    void UpdateHealthBar()
    {
        float normalizedHealth = currentHealth / maxHealth;
        healthSlider.value = normalizedHealth;
    }

    void UpdateManaBar()
    {
        float normalizedMana = currentMana / maxMana;
        manaSlider.value = normalizedMana;
    }

    void UpdateEnduranceBar()
    {
        float normalizedEndurance = currentEndurance / maxEndurance;
        enduranceSlider.value = normalizedEndurance;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UpdateHealthBar();
    }

    void UseMana(float manaCost)
    {
        currentMana -= manaCost;
        currentMana = Mathf.Max(currentMana, 0f);
        UpdateManaBar();
    }

    void UseEndurance(float enduranceCost)
    {
        currentEndurance -= enduranceCost;
        currentEndurance = Mathf.Max(currentEndurance, 0f);
        UpdateEnduranceBar();
    }

    public void SwordAttack()
    {
        swordAttack.Attack(spriteRenderer.flipX);
    }

    public void EndSwordAttack()
    {
        swordAttack.StopAttack();
    }
}
