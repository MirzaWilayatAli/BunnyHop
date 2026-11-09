// PHOEBE FAITH (1033478)
// PLAYERCONTROLLER.CS

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
    public PlayerState PlayerState => _playerState;
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

    // If for some reason we have to change the raycast parameters?
    public void SetCastParameters(Vector3 parameters) 
    {
        castParams = parameters;
    }

    public void SetCastParameters(Vector3 parameters, Vector3 offset) 
    {
        castParams = parameters;
        castOffset = offset;
    }

    private void SetPlayerState(PlayerState state)
    {
        // If PlayerState is the same as what we're trying to set, don't do that
        if (_playerState == state) return;
        BaseStateEventArgs args = new ()
        {
            player = this
        };
        // Check if already existing state
        if (_playerState != null)
        {
            // If so, then exit it! We're changing now.

            _playerState.Exit(args);
        }
        
        // Set new state and enter it
        _playerState = state;
        _playerState.Enter(args);
    }
    
    private void Awake()
    {
        // Set origin.
        _origin = transform.position;
        
        // Get components.
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            // missing Rigidbody2D, this shouldn't ever happen but like.. you know.. it's good to be sure
            throw new Exception("PlayerController: Rigidbody2D is null");
        }

        collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            // missing Collider2D, this shouldn't ever happen but like.. you know.. it's good to be sure
            throw new Exception("PlayerController: Collider2D is null");
        }
        
        SetPlayerState(_idleState);
        StartCoroutine(WaitForGameManager());
    }

    // Thanks Wilayat!
    private IEnumerator WaitForGameManager()
    {
        while (GameManager.Instance == null)
            yield return null;

        // Set player instance.
        if (GameManager.Instance)
        {
            Logger.Log(this, $"Set player instance.");
            GameManager.Instance.SetPlayer(this);
        }
    }

    public void ResetPlayer()
    {
        Logger.Log(this, $"Reset player.");

        // Reset to origin
        // The player never actually moves, but this is just if they're like,
        // mid-jump or got stuck on something when they died.
        if(animator.GetBool("isRunning") == false) animator.SetBool("isRunning", true);
        transform.position = _origin;
        isAlive = true;
    }

    public bool IsGrounded()
    {
        // Box raycast downwards to check if touching ground
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

        _playerState.Execute(args);

        // Check for falling first
        if (rb2d.velocity.y < -0.1f)
        {
            SetPlayerState(_fallingState);
        }
        // Then check for jump input
        else if (Input.GetMouseButtonDown(0) && IsGrounded()) // Changed to GetMouseButtonDown
        {
            SetPlayerState(_jumpState);
            SoundManager.Instance.PlayJumpSFX();
        }
        // Only go to idle if not jumping or falling
        else if (Mathf.Abs(rb2d.velocity.y) < 0.1f && IsGrounded())
        {
            SetPlayerState(_idleState);
        }
    }

    private void OnDrawGizmos()
    {
        // just to get a feel for the ground collision stuffs
#if UNITY_EDITOR
        Gizmos.DrawWireCube(transform.position - castOffset, castParams);
#endif
    }
}
