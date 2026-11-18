using BunnyHop.Audio;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Control states
    private readonly JumpState _jumpState = new JumpState();
    private readonly IdleState _idleState = new IdleState();
    private readonly FallingState _fallingState = new FallingState();
    private PlayerState _playerState;
    
    // Components
    [SerializeField] internal Rigidbody2D rb2d;
    [SerializeField] private new Collider2D collider2D;
    
    // Cast
    [Tooltip("Set this to the same Layer as your ground, or anything it should consider the ground.")]
    [SerializeField] private LayerMask layerMask;
    [Tooltip("x/y = width/height, z = distance")]
    [SerializeField] private Vector3 castParams = new Vector3(1, 1, 0.1f);
    [Tooltip("Offset of cast. Higher is lower!")]
    [SerializeField] private Vector3 castOffset = new Vector3(0, 0.1f, 0);
    
    // Original pos
    private Vector3 _origin;
    
    // Public variables
    [Tooltip("How much force goes into a jump: higher increases force dramatically!")]
    public float jumpForce = 6f;
    public float slamForce = 7f;
    public bool isAlive = true;
    public Animator animator;
    public bool hasSlammed;

    [Header("Jump feel")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float riseGravityScale = 2f;
    [SerializeField] private float fallGravityScale = 3.5f;

    private float _jumpBufferCounter;

    private void SetPlayerState(PlayerState state)
    {
        if (_playerState == state) return;
        BaseStateEventArgs args = new ()
        {
            player = this
        };
   
        if (_playerState != null)
        { 
            _playerState.Exit(args);
        }
        
        _playerState = state;
        _playerState.Enter(args);
    }
    
    private void Awake()
    {
        _origin = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            throw new Exception("PlayerController: Rigidbody2D is null");
        }

        collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            throw new Exception("PlayerController: Collider2D is null");
        }
        
        SetPlayerState(_idleState);
        StartCoroutine(WaitForGameManager());
    }
    
    private IEnumerator WaitForGameManager()
    {
        while (GameManager.Instance == null)
            yield return null;
        
        if (GameManager.Instance)
        {
            Logger.Log(this, $"Set player instance.");
            GameManager.Instance.SetPlayer(this);
        }
    }

    public void ResetPlayer()
    {
        Logger.Log(this, $"Reset player.");
        
        if(animator.GetBool("isRunning") == false) animator.SetBool("isRunning", true);
        transform.position = _origin;
        isAlive = true;
    }

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position - castOffset, new Vector2(castParams.x, castParams.y), 0, -transform.up, castParams.z, layerMask) == true;
    }

    private void Update()
    {
        if (!isAlive)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        BaseStateEventArgs args = new()
        {
            player = this
        };

        if (Input.GetMouseButtonDown(0))
        {
            if (IsGrounded() || hasSlammed)
                _jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        bool grounded = IsGrounded();
        
        rb2d.gravityScale = rb2d.velocity.y < 0f ? fallGravityScale : riseGravityScale;
        
        if (_jumpBufferCounter > 0f && grounded)
        {
            _jumpBufferCounter = 0f;
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
            SetPlayerState(_jumpState);
            SoundManager.Instance.PlayJumpSFX();
        }
        else
        {
            _playerState.Execute(args);

            if (rb2d.velocity.y < -0.1f && !grounded)
                SetPlayerState(_fallingState);
            else if (Mathf.Abs(rb2d.velocity.y) < 0.1f && grounded)
                SetPlayerState(_idleState);
        }
    }
}
