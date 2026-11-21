using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Gun Attack Settings")]
    [SerializeField] private float gunAttackCooldown = 0.5f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int maxAmmoCount = 10;
    [SerializeField] private Vector2 gunSpawnRightOffset = new Vector2(1.25f, 0f);
    [SerializeField] private Vector2 gunSpawnLeftOffset = new Vector2(-1.25f, 0f);
    [SerializeField] private Vector2 gunSpawnUpOffset = new Vector2(0f, 1.5f);
    [SerializeField] private Vector2 gunSpawnDownOffset = new Vector2(0f, -1.5f);
    [SerializeField] private Quaternion gunSpawnRightRotation = new Quaternion(0, 0, 0, 0);
    [SerializeField] private Quaternion gunSpawnUpRightRotation = new Quaternion(0, 0, 45, 0);
    [SerializeField] private Quaternion gunSpawnDownRightRotation = new Quaternion(0, 0, -45, 0);
    [SerializeField] private Quaternion gunSpawnLeftRotation = new Quaternion(0, 0, 180, 0);
    [SerializeField] private Quaternion gunSpawnUpLeftRotation = new Quaternion(0, 0, 135, 0);
    [SerializeField] private Quaternion gunSpawnDownLeftRotation = new Quaternion(0, 0, -135, 0);
    [SerializeField] private Quaternion gunSpawnUpRotation = new Quaternion(0, 0, 90, 0);
    [SerializeField] private Quaternion gunSpawnDownRotation = new Quaternion(0, 0, -90, 0);

    private AttackHitbox meleeAttackHitbox;
    private PlayerMovement playerMovement;
    private GroundCheck groundCheck;
    private PlayerBulletSpawnpoint bulletSpawnpoint;
    private float meleeAttackCooldownTimer = 0f;
    private float gunAttackCooldownTimer = 0f;
    private bool isOnMeleeCooldown = false;
    private bool isOnGunCooldown = false;
    private bool appliedKnockback = false;
    private int currentAmmoCount;
    private LookDirection lookDirection;
    private PlayerState currentState;
    public UnityEvent<int> UpdateAmmo;

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
        if (bulletSpawnpoint == null)
        {
            bulletSpawnpoint = GetComponentInChildren<PlayerBulletSpawnpoint>();
        }
        meleeAttackHitbox.gameObject.SetActive(false); // Ensure the attack hitbox is initially disabled
        currentAmmoCount = maxAmmoCount;
        UpdateAmmo.Invoke(currentAmmoCount);
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
        if (value.isPressed && !isOnGunCooldown && (currentAmmoCount > 0))
        {
            GunAttack();
        }
    }

    private void Update()
    {
        lookDirection = playerMovement.GetLookDirection();
        currentState = playerMovement.GetPlayerState();
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

        if (currentState == PlayerState.Shooting && !appliedKnockback)
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
        if (isOnGunCooldown)
        {
            gunAttackCooldownTimer -= Time.deltaTime;
            if (gunAttackCooldownTimer <= 0f)
            {
                isOnGunCooldown = false;
                gunAttackCooldownTimer = gunAttackCooldown; // Reset cooldown
            }
        }
    }

    private void MeleeAttack()
    {
        if (isOnMeleeCooldown) return;
        playerMovement.SetPlayerState(PlayerState.Attacking); // Set player state to attacking
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
        if (isOnGunCooldown) return;
        playerMovement.SetPlayerState(PlayerState.Shooting); // Set player state to attacking
        isOnGunCooldown = true;
        if (lookDirection == LookDirection.Down && groundCheck.IsGrounded())
        {
            if (playerMovement.WasMovingRight())
            {
                bulletSpawnpoint.ChangeSpawn(gunSpawnRightOffset, gunSpawnRightRotation);
            }
            else
            {
                bulletSpawnpoint.ChangeSpawn(gunSpawnLeftOffset, gunSpawnLeftRotation);
            }
        }
        else
        {
            switch (lookDirection)
            {
                case LookDirection.Right:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnRightOffset, gunSpawnRightRotation);
                    break;
                case LookDirection.Left:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnLeftOffset, gunSpawnLeftRotation);
                    break;
                case LookDirection.Up:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnUpOffset, gunSpawnUpRotation);
                    break;
                case LookDirection.UpRight:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnRightOffset, gunSpawnUpRightRotation);
                    break;
                case LookDirection.UpLeft:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnLeftOffset, gunSpawnUpLeftRotation);
                    break;
                case LookDirection.Down:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnDownOffset, gunSpawnDownRotation);
                    break;
                case LookDirection.DownRight:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnRightOffset, gunSpawnDownRightRotation);
                    break;
                case LookDirection.DownLeft:
                    bulletSpawnpoint.ChangeSpawn(gunSpawnLeftOffset, gunSpawnDownLeftRotation);
                    break;
            }
        }
        bulletSpawnpoint.SpawnBullet(bulletSpeed); // Shoot bullet in the direction the player is looking
        currentAmmoCount--;
        UpdateAmmo.Invoke(currentAmmoCount);
        Invoke("ResetGunAttack", gunAttackCooldown);
    }

    private void ResetMeleeAttack()
    {
        playerMovement.SetPlayerState(PlayerState.Idle); // Reset player state to idle after attack
        meleeAttackHitbox.gameObject.SetActive(false);
        appliedKnockback = false; // Reset knockback application for the next attack
    }

    private void ResetGunAttack()
    {
        playerMovement.SetPlayerState(PlayerState.Idle); // Reset player state to idle after attack
        appliedKnockback = false; // Reset knockback application for the next attack
    }

    public void RestoreAmmo(int ammo)
    {
        currentAmmoCount += ammo;
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
