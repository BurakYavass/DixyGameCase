using System;
using Cinemachine;
using UnityEngine;


public class PlayerController : ObjectID
{
    public static PlayerController current;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Joystick joystick;
    private CharacterController _characterController;

    //private float _currentMoveMultiplier;
    
    [SerializeField]public float speed = 2f;
    [SerializeField]private float turnSpeed;
    //[SerializeField]private float _acceleration;

    public bool walking = false;
    private bool once = false;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        var inputVector = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (joystick.Horizontal != 0f || joystick.Vertical != 0f)
        {
            if (!once)
            {
                walking = true;
                once = true;
            }
        }
        else
        {
            walking = false;
            once = false;
        }

        var cameraTransform = virtualCamera.transform;
        var forward = cameraTransform.forward;
        var cameraForwardHorizontal =
            new Vector3(forward.x, 0f, forward.z).normalized;

        var right = cameraTransform.right;
        var cameraRightHorizontal =
            new Vector3(right.x, 0f, right.z).normalized;

        var movementVector = inputVector.x * cameraRightHorizontal + inputVector.z * cameraForwardHorizontal;

        //var dot = Mathf.Clamp(Vector3.Dot(transform.forward, inputVector),0,1);

        //_currentMoveMultiplier = Mathf.Lerp(_currentMoveMultiplier, dot, _acceleration * Time.fixedDeltaTime);
        
        //var position = transform.position + transform.forward.normalized * (speed * dot * Time.fixedDeltaTime);

        //transform.position += movementVector.normalized * (speed * Time.deltaTime);
        _characterController.Move(movementVector.normalized * (speed * Time.deltaTime));

        if (inputVector.magnitude > 0)
        {
            var newRotation = Quaternion.LookRotation(movementVector, Vector3.up);
        
            var rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.fixedDeltaTime);
            
            transform.rotation = rotation;
        }
    }
    
}
