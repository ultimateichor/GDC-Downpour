using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Melee Attack Settings")]
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float meleeAttackCooldown = 1f;
    [SerializeField] private Vector2 meleeColliderHorizontalSize = new Vector2(1.5f, 0.5f);
    [SerializeField] private Vector2 meleeColliderVerticalSize = new Vector2(0.5f, 1.5f);
    [SerializeField] private Vector2 meleeColliderRightOffset = new Vector2(1.25f, 0f);
    [SerializeField] private Vector2 meleeColliderLeftOffset = new Vector2(-1.25f, 0f);
    [SerializeField] private Vector2 meleeColliderUpwardOffset = new Vector2(0f, 1.5f);
    [SerializeField] private Vector2 meleeColliderDownwardOffset = new Vector2(0f, -1.5f);

    private AttackHitbox meleeAttackHitbox;
    private PlayerMovement playerMovement;
    private GroundCheck groundCheck;
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
        if (groundCheck == null)
        {
            groundCheck = GetComponentInChildren<GroundCheck>();
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
        lookDirection = playerMovement.GetLookDirection();
        // apply knockback if melee attack is successful
        if (meleeAttackHitbox.gameObject.activeSelf && meleeAttackHitbox.IsHittingSomething() && !appliedKnockback)
        {
            if (lookDirection == LookDirection.Right || lookDirection == LookDirection.UpRight || lookDirection == LookDirection.DownRight)
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.left);
            }
            else if (lookDirection == LookDirection.Left || lookDirection == LookDirection.UpLeft || lookDirection == LookDirection.DownLeft)
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.right);
            }
            else if (lookDirection == LookDirection.Up)
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.down);
            }
            else if (lookDirection == LookDirection.Down)
            {
                playerMovement.ApplyKnockback(knockbackForce, Vector2.up);
            }
            appliedKnockback = true;
            //Debug.Log(lookDirection);
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
        if (lookDirection == LookDirection.Down && groundCheck.IsGrounded())
        {
            if (playerMovement.WasMovingRight())
            {
                meleeAttackHitbox.ChangeHitbox(meleeColliderHorizontalSize, meleeColliderRightOffset);
            }
            else
            {
                meleeAttackHitbox.ChangeHitbox(meleeColliderHorizontalSize, meleeColliderLeftOffset);
            }
        }
        else
        {
            switch (lookDirection)
            {
                case LookDirection.Right:
                    meleeAttackHitbox.ChangeHitbox(meleeColliderHorizontalSize, meleeColliderRightOffset);
                    break;
                case LookDirection.Left:
                    meleeAttackHitbox.ChangeHitbox(meleeColliderHorizontalSize, meleeColliderLeftOffset);
                    break;
                case LookDirection.Up:
                case LookDirection.UpRight:
                case LookDirection.UpLeft:
                    meleeAttackHitbox.ChangeHitbox(meleeColliderVerticalSize, meleeColliderUpwardOffset);
                    break;
                case LookDirection.Down:
                case LookDirection.DownRight:
                case LookDirection.DownLeft:
                    meleeAttackHitbox.ChangeHitbox(meleeColliderVerticalSize, meleeColliderDownwardOffset);
                    break;
            }
        }
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
