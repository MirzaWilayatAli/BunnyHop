// PHOEBE FAITH (1033478)
// SCROLLER.CS

// mostly unused it seems </3
// oh well.
using System;
using System.Collections;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    // Original pos
    [SerializeField] private Vector3 origin;
    [SerializeField] private Vector2 resetXPositions;
    // Public variables (and event)
    [Tooltip("Currently scrolling")]
    public bool isScrolling;
    [Tooltip("Direction of scroll.")]
    public bool scrollToLeft = true;
    [Tooltip("Active speed of scroll")]
    public float speed = 1.0f;
    public Action OnScroll;

    // Thanks Wilayat!
    // You're most welcome
    private IEnumerator WaitForGameManager()
    {
        while (GameManager.Instance == null)
            yield return null;
   
        // Set scroller instance
        if (GameManager.Instance)
        {
            Logger.Log(this, $"Set scroller instance.");
            GameManager.Instance.SetScroller(this);
        }    
    }
    
    private void Awake()
    {
        // Set origin
        origin = transform.position;
        StartCoroutine(WaitForGameManager());
    }

    public void ResetScroller()
    {
        Logger.Log(this, $"Reset scroller.");

        OnScroll?.Invoke();
        // Reset to origin
        transform.position = origin;
        isScrolling = true;
    }
    
    void Update()
    {
        if (transform.position.x <= resetXPositions.x || transform.position.y >= resetXPositions.y) // resetXPositions holds 2 X values to reset back to origin if hit
        {
            Logger.Log(this, $"Reset scroller to origin.");
            
            OnScroll?.Invoke();
            // Reset to origin
            transform.position = origin;
        }
        else
        {
            if (isScrolling)
            {
                // Actually scroll, direction can be chosen by modifying
                // scrollToLeft. Could be used for a cool modifier maybe?
                transform.position += (scrollToLeft ? Vector3.left : Vector3.right) * (Time.deltaTime * speed);
            } 
        }
    }
}
