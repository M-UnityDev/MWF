using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;
using System;
public class Movement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool IsSecondPlayer;
    [Header("Parameters")]
    [SerializeField] private int BaseSpeed;
    [SerializeField] private int SprintSpeed;
    [SerializeField] private int JumpHeight;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private int DistanceToWalkSqrt;
    public int DistanceToWalkF {get => DistanceToWalk; set => DistanceToWalk = value;}
    [SerializeField] private int DistanceToCheck;
    [SerializeField] private bool IsFPC;
    [Header("GameObjects")]
    [SerializeField] private Transform AnotherPlayer;
    [SerializeField] private CinemachineCamera Camera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin CameraShake;
    [SerializeField] private GameObject FPC;
    [SerializeField] private Transform CameraTransform;
    [Header("Debug Values")]
    private CharacterController CharacterControl;
    private int CurrentSpeed;
    private Vector3 Velocity;
    private Vector3 MoveVector;
    private bool TempMove;
    private InputAction Move;
    private InputAction Interact;
    private InputAction Sprint;
    private InputAction Jump;
    private bool IsClose;
    private Collider[] Colliders;
    private void Start()
    {
        Move = IsSecondPlayer ? InputHandler.Inputs.Player.MovePlayer2 : InputHandler.Inputs.Player.MovePlayer1;
        Jump = IsSecondPlayer ? InputHandler.Inputs.Player.JumpPlayer2 : InputHandler.Inputs.Player.JumpPlayer1; 
        Interact = IsSecondPlayer ? InputHandler.Inputs.Player.InteractPlayer2 : InputHandler.Inputs.Player.InteractPlayer1;
        Sprint = IsSecondPlayer ? InputHandler.Inputs.Player.SprintPlayer2 : InputHandler.Inputs.Player.SprintPlayer1;
        CharacterControl = GetComponent<CharacterController>();
    }
    private void Update()
    {
        UpdateJump();
        UpdateSpeedChanges();
        UpdatePhysics();
        UpdateCollisons();
        if (IsFPC)
            FPCCameraAnimation();
        else
            transform.localRotation = Quaternion.Lerp(transform.rotation, new Vector3(Move.ReadValue<Vector2>().x, 0, Move.ReadValue<Vector2>().y).sqrMagnitude > 0 ? Quaternion.LookRotation(new Vector3(Move.ReadValue<Vector2>().x, 0, Move.ReadValue<Vector2>().y)) : transform.rotation, RotationSpeed * Time.deltaTime);
        //UpdateSound();
    }
    private void FPCCameraAnimation()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
        TempMove = !Move.ReadValue<Vector2>().Equals(Vector2.zero);
        if (Sprint.IsPressed() && TempMove)
            CameraShake.AmplitudeGain = 1;
        else if (TempMove)
            CameraShake.AmplitudeGain = 0.25f;
        else if (!TempMove)
            CameraShake.AmplitudeGain = 0;
    }
    private void UpdateSpeedChanges()
    {
        IsClose = Vector3.SqrtDistance(transform.position, AnotherPlayer.position) <= DistanceToWalkSqrt || IsFPC;
        if (Sprint.IsPressed() && IsClose)
        {
            CurrentSpeed = SprintSpeed;
            //Footstep.pitch = 2;
        }
        else if (IsClose)
        {
            CurrentSpeed = BaseSpeed;
            //Footstep.pitch = 1;
        }
        else
        {
            CurrentSpeed = 1;
            //Footstep.pitch = 1;
        }
    }
    private void UpdateJump()
    {
        if (Jump.IsPressed() && CharacterControl.isGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
            //GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }
        if (CharacterControl.isGrounded && Velocity.y < 0)
            Velocity.y = -2;
    }
    //private void UpdateSound()
    //{
        //if (CharacterControl.isGrounded && Move.ReadValue<Vector2>() != Vector2.zero)
            //Footstep.mute = false;
        //else
            //Footstep.mute = true;
    //}
    private void UpdatePhysics()
    {
        MoveVector = IsFPC ? transform.right * Move.ReadValue<Vector2>().x + transform.forward * Move.ReadValue<Vector2>().y : new Vector3(Move.ReadValue<Vector2>().x, 0, Move.ReadValue<Vector2>().y);
        CharacterControl.Move(CurrentSpeed * Time.deltaTime * MoveVector);
        Velocity.y += Physics.gravity.y * Time.deltaTime;
        CharacterControl.Move(Velocity * Time.deltaTime);
    }
    private void UpdateCollisons()
    {
        Colliders = Physics.OverlapBox(transform.position + transform.forward * DistanceToCheck, Vector3.one / 2, transform.localRotation);
        foreach (Collider Item in Colliders)
        {
            if (Item.CompareTag("Finish"))
            {
                Destroy(Item.gameObject);
                FPC.SetActive(true);
                IsFPC = true;
                GetComponent<MeshRenderer>().enabled = false;
                AnotherPlayer.GetComponent<Movement>().DistanceToWalkF = int.MaxValue;
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            else if (Item.GetComponent<Transform>().Equals(AnotherPlayer) && IsFPC)
            {
                Destroy(Item.gameObject);
                AnotherPlayer = FPC.transform;
                return;
            }
        }
    }
}
