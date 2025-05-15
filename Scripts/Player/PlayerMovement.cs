using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides actions for the player based on the input.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Represents the <see cref="CharacterController2d"/> component attached to the player.
    /// </summary>
    public CharacterController2d controller;

    /// <summary>
    /// Represents the <see cref="Animator"/> component attached to the player.
    /// </summary>
    public Animator animator;

    public float runSpeed = 40f;
    public float rollSpeed = 60f;
    public float rollDuration = 0.5f;

    public int currElem = 0;
    public int selectedElem = 0;
    
    private float attackTimeTracker = 0.0f;
    private float _horizontalMovement = 0.0f;
    private bool _jump = false;
    private bool _crouch = false;
    private bool _roll = false;
    private bool _attack = false;
    private bool _eAttack = false;
    private bool inCombat = false;
    public bool isDead = false;

    public InventoryManager inventoryManager;

    [SerializeField]
    private GameObject[] AvailableSpells;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (inCombat)
            {
                StopMovement();
                return;
            }

            _horizontalMovement = Input.GetAxisRaw(Axis.Horizontal) * runSpeed;
            animator.SetFloat(Animators.Player.Speed, Mathf.Abs(_horizontalMovement));

            // Check for jump
            if (Input.GetButtonDown(Buttons.Jump))
            {
                _jump = true;
                controller._canDoubleJump = inventoryManager == null ? true : inventoryManager.doubleJump;
            }

            // Check for crouch
            if (Input.GetButtonDown(Buttons.Crouch))
            {
                _crouch = true;
            }
            else if (Input.GetButtonUp(Buttons.Crouch))
            {
                _crouch = false;
            }

            // Check for roll
            if (Input.GetButtonDown(Buttons.Roll))
            {
                if (gameObject.GetComponent<Player>().currentEndurance >= 25)
                {
                    _roll = true;
                }
            }

            if (Input.GetButtonDown(Buttons.Fire1))
            {
                _attack = true;
            }

            if (Input.GetButtonDown(Buttons.Fire2))
            {
                _eAttack = true;
            }

            if (Input.GetButtonDown(Buttons.SelectNextElem))
            {
                SelectElem(1);
            }

            if (Input.GetButtonDown(Buttons.SelectPrevElem))
            {
                SelectElem(-1);
            }
        }
    }

    private void FixedUpdate()
    {
        if (attackTimeTracker > 0.0f)
        {
            if (_roll || _jump) transform.Find("Hitbox").GetComponent<PolygonCollider2D>().enabled = false; //If player rolls or jumps during an attack, disable its hitbox
            attackTimeTracker -= Time.deltaTime;
            if (attackTimeTracker <= 0.0f)
            {
                attackTimeTracker = 0.0f;
            }
        }
        if (_attack && controller.GetGrounded() && attackTimeTracker == 0.0f)
        {
            currElem = 0;
            controller.Attack();
            attackTimeTracker = 0.5f;
        }
        else if (_eAttack && controller.GetGrounded() && attackTimeTracker == 0.0f)
        {
            currElem = selectedElem;
            controller.ElemAttack();
            attackTimeTracker = 0.5f;
        }
        else if (_roll && controller.GetGrounded())
        {
            controller.Roll(rollSpeed, rollDuration);
        }
        else
        {
            controller.Move(_horizontalMovement * Time.fixedDeltaTime, _crouch, _jump);
            _attack = false;
            _eAttack = false;
            _roll = false;
            _jump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(true, currElem);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(false, currElem);
        }
    }

    public void SpawnSpellEffect()
    {
        if (currElem <= 0 || AvailableSpells[currElem - 1] == null) return;
        GameObject spell = Instantiate(AvailableSpells[currElem - 1]);
        spell.transform.position = transform.Find("SpellSpawnPoint").transform.position;
        if (!GetComponent<CharacterController2d>().IsFlipped())
        {
            var theScale = spell.transform.localScale;
            theScale.x *= -1;
            spell.transform.localScale = theScale;
        }
    }

    public void OnJumping()
    {
        animator.SetBool(Animators.Player.IsJumping, true);
    }

    public void OnLanding()
    {
        animator.SetBool(Animators.Player.IsJumping, false);
        animator.SetBool(Animators.Player.IsFalling, false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool(Animators.Player.IsCrouching, isCrouching);
    }

    public void OnRolling(bool isRolling)
    {
        animator.SetBool(Animators.Player.IsRolling, isRolling);
        if (isRolling)
        {
            _roll = false;
        }
    }

    public void OnFalling(bool isFalling)
    {
        animator.SetBool(Animators.Player.IsFalling, isFalling);
    }

    public void OnAttacking(bool isAttacking)
    {
        animator.SetTrigger(Animators.Player.IsAttacking);
        if (isAttacking)
        {
            _attack = false;
        }
    }

    public void OnElemAttacking(bool isEAttacking)
    {
        animator.SetTrigger(Animators.Player.IsElemAttacking);
        animator.SetInteger(Animators.Player.CurrElem, currElem);
        if (isEAttacking)
        {
            _eAttack = false;
        }
    }

    public void SelectElem(int direction)
    {
        // 1. Construire la liste des éléments débloqués
        List<int> unlockedElements = new List<int>();
        unlockedElements.Add(0); // Élement "normal"
        if (inventoryManager.firePower) unlockedElements.Add(1);
        if (inventoryManager.windPower) unlockedElements.Add(2);
        if (inventoryManager.icePower) unlockedElements.Add(3);
        if (inventoryManager.earthPower) unlockedElements.Add(4);

        // Si la liste contient 0 ou 1 élément, on n’a rien à faire
        if (unlockedElements.Count <= 1)
        {
            // On s’assure que selectedElem=0 si on veut être cohérent
            selectedElem = 0;
            currElem = 0;
            return;
        }

        // 2. Récupérer l’index actuel du currElem dans la liste
        int index = unlockedElements.IndexOf(currElem);

        // 3. S’il n’est pas trouvé (retourne -1), on se rabat sur l’index 0
        if (index < 0)
        {
            index = 0;
            currElem = unlockedElements[index];
        }

        // 4. Ajuster l’index de sélection
        index += direction;

        // 5. Gestion du “wrap” (on revient au début si on dépasse la fin, etc.)
        if (index < 0)
        {
            index = unlockedElements.Count - 1;
        }
        else if (index >= unlockedElements.Count)
        {
            index = 0;
        }

        // 6. Mettre à jour nos variables
        currElem = unlockedElements[index];
        selectedElem = currElem;
    }

    public void ResetElemOnAttackFinish()
    {
        currElem = 0;
    }

    public void OnPlayerHit(Vector2 knockbackForce)
    {
        Debug.Log("Knockback force: " + knockbackForce);
        animator.SetTrigger("Player_Hit");

        // Get the player's Rigidbody2D component
        // Apply knockback force to the player's Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }

    public void CombatTrigger(bool state)
    {
        inCombat = state;
        gameObject.GetComponent<Player>().isInCombat = state;
    }

    public bool IsInCombat()
    {
        return inCombat;
    }

    public void StopMovement()
    {
        _horizontalMovement = 0;
        animator.SetFloat(Animators.Player.Speed, 0);
    }

    public void Death()
    {
        isDead = true;
        StopMovement();
    }
}
