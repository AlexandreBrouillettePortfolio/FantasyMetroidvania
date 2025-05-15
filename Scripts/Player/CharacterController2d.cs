using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides character control for a 2D platformer player.
/// </summary>
/// <remarks>
/// Adapted from Brackeys' 2D Platformer Character Controller tutorial.
/// <see href="https://github.com/Brackeys/2D-Character-Controller/blob/master/CharacterController2D.cs"/>
/// No license was provided with the original code, but the file is free.
/// </remarks>
public class CharacterController2d : MonoBehaviour
{
    /// <summary>
    /// The amount of force added when the player jumps.
    /// </summary>
    [SerializeField]
    private float _jumpForce = 400f;

    /// <summary>
    /// The amount of maxSpeed applied to crouching movement. 1 = 100%.
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    private float _crouchSpeed = .36f;

    /// <summary>
    /// Specify how much to smooth out the movement.
    /// </summary>
    [Range(0, .3f)]
    [SerializeField]
    private float _movementSmoothing = .05f;

    /// <summary>
    /// Whether or not a player can steer while jumping;
    /// </summary>
    [SerializeField]
    private bool _airControl = false;

    /// <summary>
    /// Whether or not a player can double jump. meaning it can jump one time while not grounded.
    /// </summary>
    [SerializeField]
    public bool _canDoubleJump = false;

    /// <summary>
    /// A mask determining what is ground to the character.
    /// </summary>
    [SerializeField]
    private LayerMask _whatIsGround;

    /// <summary>
    /// A position marking where to check if the player is grounded.
    /// </summary>
    [SerializeField]
    private Transform _groundCheck;

    /// <summary>
    /// Radius of the overlap circle to determine if grounded
    /// </summary>
    [SerializeField]
    private float _groundCheckRadius = .2f;

    /// <summary>
    /// The amount of time to wait before checking if the player is grounded after jumping.
    /// </summary>
    [SerializeField]
    private float _jumpGroundCheckDelay = 0.05f;

    /// <summary>
    /// The amount of time after last touching ground where the player can still jump
    /// </summary>
    [SerializeField]
    private float _coyoteTimeDelay = 1.0f;

    /// <summary>
    /// A position marking where to check for ceilings
    /// </summary>
    [SerializeField]
    private Transform _ceilingCheck;

    /// <summary>
    /// Radius of the overlap circle to determine if the player can stand up
    /// </summary>
    [SerializeField]
    private float _ceilingCheckRadius = .2f;

    /// <summary>
    /// A collider that will be disabled when crouching
    /// </summary>
    [SerializeField]
    private Collider2D _crouchDisableCollider;

    [SerializeField] private Player player;

    /// <summary>
    /// The collider that should be enabled when attacking.
    /// </summary>
    [SerializeField]
    private Collider2D _attackCollider;

    private bool _grounded;            // Whether or not the player is grounded.
    private bool _doubleJumped = false; // Whether or not the player has double jumped.
    private Rigidbody2D _rigidbody2D;
    private bool _facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 _velocity = Vector3.zero;
    private float _jumpTime = 0f;
    private float _lastGroundedTime = 0f;
    private bool _canCoyoteJump = false;

    /// <summary>
    /// Triggers when the player jumps.
    /// </summary>
    [Header("Events")]
    [Space]
    public UnityEvent OnJumpEvent;

    /// <summary>
    /// Triggers when the player lands on the ground.
    /// </summary>
    [Space]
    public UnityEvent OnLandEvent;

    /// <summary>
    /// Triggers when the player crouches or stands up.
    /// </summary>
    public BoolEvent OnCrouchEvent;
    private bool _wasCrouching = false;

    /// <summary>
    /// Triggers when the player rolls.
    /// </summary>
    public BoolEvent OnRollEvent;
    private bool _isRolling = false;

    /// <summary>
    /// Triggers when the player falls from a ledge or after jumping.
    /// </summary>
    public BoolEvent OnFallEvent;
    private bool _isFalling = false;

    /// <summary>
    /// Triggers when the player uses a basic attack.
    /// </summary>
    public BoolEvent OnAttackEvent;
    private bool _isAttacking = false;

    /// <summary>
    /// Triggers when the player uses an elemental attack.
    /// </summary>
    public BoolEvent OnElemAttackEvent;
    private bool _isElemAttacking = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        OnJumpEvent ??= new UnityEvent();
        OnLandEvent ??= new UnityEvent();
        OnCrouchEvent ??= new BoolEvent();
        OnRollEvent ??= new BoolEvent();
        OnFallEvent ??= new BoolEvent();
        OnAttackEvent ??= new BoolEvent();
        OnElemAttackEvent ??= new BoolEvent();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        Gizmos.DrawWireSphere(_ceilingCheck.position, _ceilingCheckRadius);
    }

    private void FixedUpdate()
    {
        var wasGrounded = _grounded;
        _grounded = false;

        if (wasGrounded)
        {
            _lastGroundedTime = Time.time;
        }

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.

        // Don't check before the jump initial delay expires
        if (Time.time - _jumpTime > _jumpGroundCheckDelay)
        {
            var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundCheckRadius, _whatIsGround);
            for (var i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    _grounded = true;
                    _doubleJumped = false;
                    if (!wasGrounded)
                        OnLandEvent.Invoke();
                }
            }
        }

        // Check if the character is falling
        if (!_grounded)
        {
            // Use Coyote Time to check if the player can still jump even if not touching ground
            if (wasGrounded && Time.time - _lastGroundedTime < _coyoteTimeDelay)
            {
                _canCoyoteJump = true;
            }
            else if (Time.time - _lastGroundedTime >= _coyoteTimeDelay)
            {
                _canCoyoteJump = false;
            }

            if (_rigidbody2D.velocity.y < 0)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                    OnFallEvent.Invoke(true);
                }
            }
            else if (_rigidbody2D.velocity.y >= 0)
            {
                if (_isFalling)
                {
                    _isFalling = false;
                    OnFallEvent.Invoke(false);
                }
            }
        }
        else
        {
            if (_isAttacking)
            {
                _isAttacking = false;
            }
            if (_isElemAttacking)
            {
                _isElemAttacking = false;
            }
            if (_isFalling)
            {
                _isFalling = false;
                OnFallEvent.Invoke(false);
            }
        }
    }

    /// <summary>
    /// Provides a way to move the player while crouching or jumping.
    /// </summary>
    /// <param name="move">The horizontal movement for the player.</param>
    /// <param name="crouch">Whether or not the player is crouching.</param>
    /// <param name="jump">Whether or not the player is jumping.</param>
    public void Move(float move, bool crouch, bool jump)
    {
        if (!_isRolling)
        {
            MoveCore(move, crouch, jump);
        }
    }

    public void Roll(float rollSpeed, float rollDuration)
    {
        if (!_isRolling && _grounded && player.currentEndurance >= 25)
        {
            player.ModifyEndurance(-25);
            _isRolling = true;
            OnRollEvent.Invoke(true);

            StartCoroutine(RollCoroutine(rollSpeed, rollDuration));
        }
    }

    private System.Collections.IEnumerator RollCoroutine(float rollSpeed, float rollDuration)
    {
        var elapsedTime = 0.0f;
        var direction = _facingRight ? 1 : -1;

        while (elapsedTime < rollDuration)
        {
            MoveCore(direction * rollSpeed * Time.fixedDeltaTime, false, false);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isRolling = false;
        OnRollEvent.Invoke(false);
    }

    public void Attack()
    {
        if (!_isRolling && !_isAttacking && _grounded)
        {
            _isAttacking = true;
            OnAttackEvent.Invoke(true);

            //StartCoroutine(AttackCoroutine(0.1f));
        }
    }

    private System.Collections.IEnumerator AttackCoroutine(float attackStartup)
    {
        var elapsedTime = 0.0f;
        var direction = _facingRight ? 1 : -1;

        while (elapsedTime < attackStartup)
        {
            //MoveCore(direction * rollSpeed * Time.fixedDeltaTime, false, false);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isAttacking = false;
        OnAttackEvent.Invoke(false);
    }

    public void ElemAttack()
    {
        if (!_isRolling && !_isElemAttacking && _grounded)
        {
            _isElemAttacking = true;
            OnElemAttackEvent.Invoke(true);

            //StartCoroutine(ElemAttackCoroutine(0.1f));
        }
    }

    private System.Collections.IEnumerator ElemAttackCoroutine(float attackStartup)
    {
        var elapsedTime = 0.0f;
        var direction = _facingRight ? 1 : -1;

        while (elapsedTime < attackStartup)
        {
            //MoveCore(direction * rollSpeed * Time.fixedDeltaTime, false, false);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isElemAttacking = false;
        OnElemAttackEvent.Invoke(false);
    }

    private void MoveCore(float move, bool crouch, bool jump)
    {
        // if a request to crouch is made while not grounded, ignore it
        if (!_grounded && crouch)
        {
            crouch = false;
        }

        // If crouching, check to see if the character can stand up
        if (_grounded && !crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(_ceilingCheck.position, _ceilingCheckRadius, _whatIsGround))
            {
                crouch = true;
                jump = false;
            }
        }

        // only control the player if grounded or airControl is turned on
        if (_grounded || _airControl)
        {
            // If crouching
            if (crouch)
            {
                if (!_wasCrouching)
                {
                    _wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= _crouchSpeed;

                // Disable one of the colliders when crouching
                if (_crouchDisableCollider != null)
                    //_crouchDisableCollider.enabled = false;
                    _crouchDisableCollider.offset = new Vector2(0, -1.50f);
            }
            else
            {
                // Enable the collider when not crouching
                if (_crouchDisableCollider != null)
                    //_crouchDisableCollider.enabled = true;
                    _crouchDisableCollider.offset = new Vector2(0, -0.76f);

                if (_wasCrouching)
                {
                    _wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, _movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            // Otherwise if the input is moving the player left and the player is facing right...
            if (move > 0 && !_facingRight || move < 0 && _facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if ((_grounded || _canCoyoteJump) && jump)
        {
            OnJumpEvent.Invoke();
            // Add a vertical force to the player.
            _grounded = false;

            if (_canCoyoteJump)
                 _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);

            _rigidbody2D.AddForce(new Vector2(0f, _jumpForce));
            _jumpTime = Time.time;
        }
        else if (!_grounded && jump && _canDoubleJump && !_doubleJumped)
        {
            _doubleJumped = true;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
            _rigidbody2D.AddForce(new Vector2(0f, _jumpForce));
        }
    }

    public bool GetGrounded()
    {
        return _grounded;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1.
        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool IsFlipped()
    {
        return _facingRight;
    }
}
