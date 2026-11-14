using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float meleeAttackCooldown = 1f;

    private AttackHitbox meleeAttackHitbox;
    private PlayerMovement playerMovement;
    private float meleeAttackCooldownTimer = 0f;
    private bool isOnMeleeCooldown = false;
    private bool isOnGunCooldown = false;
    private bool appliedKnockback = false;
    private LookDirection lookDirection;
    private PlayerState currentState;

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
        if (value.isPressed && !isOnMeleeCooldown)
        {
            MeleeAttack();
        }
    }

    private void OnShoot(InputValue value)
    {
        if (value.isPressed && !isOnGunCooldown)
        {
            GunAttack();
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
            else //if (LookDirection == lookDirection.Left)
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.right);
            }
            //else if (LookDirection == lookDirection.Up)
            //{
            //    playerMovement.ApplyKnockback(knockbackForce, Vector2.down);
            //}
            //else if (LookDirection == lookDirection.Down)
            //{
            //    playerMovement.ApplyKnockback(knockbackForce, Vector2.up);
            //}
            appliedKnockback = true;
            Debug.Log(lookDirection);
        }

        if (isOnMeleeCooldown)
        {
            meleeAttackCooldownTimer -= Time.deltaTime;
            if (meleeAttackCooldownTimer <= 0f)
            {
                isOnMeleeCooldown = false;
                meleeAttackCooldownTimer = meleeAttackCooldown; // Reset cooldown
            }
        }
    }

    private void MeleeAttack()
    {
        meleeAttackHitbox.gameObject.SetActive(true);
        isOnMeleeCooldown = true;
        Invoke("ResetMeleeAttack", attackDuration);
    }

    private void GunAttack()
    {
        // spawn bullet based on the player's current look direction
    }

    private void ResetMeleeAttack()
    {
        meleeAttackHitbox.gameObject.SetActive(false);
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
