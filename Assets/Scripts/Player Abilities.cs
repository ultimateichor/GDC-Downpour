using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float attackCooldown = 1f;

    private AttackHitbox meleeAttackHitbox;
    private PlayerMovement playerMovement;
    private float attackCooldownTimer = 0f;
    private bool isOnCooldown = false;
    private bool appliedKnockback = false;

    private void Start()
    {
        if (meleeAttackHitbox == null)
        {
            meleeAttackHitbox = GetComponentInChildren<AttackHitbox>();
        }
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        meleeAttackHitbox.gameObject.SetActive(false); // Ensure the attack hitbox is initially disabled
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed && !isOnCooldown)
        {
            MeleeAttack();
        }
    }

    private void Update()
    {
        // apply knockback if melee attack is successful
        if (meleeAttackHitbox.gameObject.activeSelf && meleeAttackHitbox.IsHittingSomething() && !appliedKnockback)
        {
            if (playerMovement.IsFacingRight())
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.left);
            }
            else
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.right);
            }
            appliedKnockback = true;
        }

        if (isOnCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                isOnCooldown = false;
                attackCooldownTimer = attackCooldown; // Reset cooldown
            }
        }
    }

    private void MeleeAttack()
    {
        meleeAttackHitbox.gameObject.SetActive(true);
        Invoke("ResetAttack", attackDuration);
    }

    private void ResetAttack()
    {
        meleeAttackHitbox.gameObject.SetActive(false);
        isOnCooldown = true;
        appliedKnockback = false; // Reset knockback application for the next attack
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackHitbox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(meleeAttackHitbox.GetComponent<Collider2D>().bounds.center, meleeAttackHitbox.GetComponent<Collider2D>().bounds.size);
        }
    }
}
