using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigibody == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
                LounchBall();

            isDragging = false;

            return;
        }

        isDragging = true;

        currentBallRigibody.isKinematic = true;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

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
