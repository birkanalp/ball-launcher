using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float detachDelay;
    [SerializeField] float respawnDelay;

    Rigidbody2D currentBallRigibody;
    SpringJoint2D currentBallSpringJoint;
    Camera mainCamera;
    bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigibody == null)
            return;

        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
                LounchBall();

            isDragging = false;

            return;
        }

        isDragging = true;

        currentBallRigibody.isKinematic = true;

        Vector2 touchPositions = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }

        touchPositions /= Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);

        // Debug.Log(worldPosition);
        currentBallRigibody.position = worldPosition;

    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigibody = ballInstance.GetComponent<Rigidbody2D>();

        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LounchBall()
    {
        currentBallRigibody.isKinematic = false;
        currentBallRigibody = null;

        Invoke(nameof(DelounchBall), detachDelay);
    }

    private void DelounchBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }


}
