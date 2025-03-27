using Game.Input;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool IsSecondPlayer;
    [Header("Parameters")]
    [SerializeField] private int BaseSpeed;
    [SerializeField] private int SprintSpeed;
    [SerializeField] private int SlowSpeed;
    [SerializeField] private int JumpHeight;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private int DistanceToWalkSqr;
    [SerializeField] private AudioClip JumpSound;
    public int DistanceToWalkFuckYou { get => DistanceToWalkSqr; set => DistanceToWalkSqr = value; }
    [SerializeField] private int DistanceToCheck;
    [SerializeField] private bool IsFPC;
    [Header("GameObjects")]
    [SerializeField] private AudioSource Footstep;
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
    private Vector3 TempMoveVector;
    private bool TempMove;
    private InputAction Move;
    private InputAction Interact;
    private InputAction Sprint;
    private InputAction Jump;
    private bool IsClose;
    private Collider[] Colliders;
    private Vector2 InputMove;
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
        ReadInput();
        UpdateJump();
        UpdateSpeedChanges();
        UpdatePhysics();
        UpdateCollisons();
        UpdateSound();
        UpdateRotation();
    }
    private void ReadInput()
    {
        InputMove = Move.ReadValue<Vector2>();
        TempMoveVector = new Vector3(InputMove.x, 0, InputMove.y);
    }
    private void UpdateJump()
    {
        if (Jump.IsPressed() && CharacterControl.isGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
            GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }
        if (CharacterControl.isGrounded && Velocity.y < 0)
            Velocity.y = -2;
    }
    private void UpdateSpeedChanges()
    {
        IsClose = Vector3.SqrMagnitude(AnotherPlayer.position - transform.position) <= DistanceToWalkSqr || IsFPC;
        if (Sprint.IsPressed() && IsClose)
            CurrentSpeed = SprintSpeed;
        else if (IsClose)
            CurrentSpeed = BaseSpeed;
        else
            CurrentSpeed = SlowSpeed;
    }
    private void UpdatePhysics()
    {
        MoveVector = IsFPC ? transform.right * TempMoveVector.x + transform.forward * TempMoveVector.z : TempMoveVector;
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
                AnotherPlayer.GetComponent<Movement>().DistanceToWalkFuckYou = int.MaxValue;
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            else if (Item.GetComponent<Transform>().Equals(AnotherPlayer) && IsFPC && Interact.WasPressedThisFrame())
            {
                Destroy(Item.gameObject);
                AnotherPlayer = FPC.transform;
                return;
            }
        }
    }
    private void UpdateSound()
    {
        if (IsFPC)
        {
            Footstep.mute = !CharacterControl.isGrounded || InputMove.Equals(Vector2.zero);
            Footstep.pitch = Jump.IsPressed() ? 2 : 1;
        }
    }
    private void UpdateRotation()
    {
        switch (IsFPC)
        {
            case true:
                FPCCameraAnimation();
                break;
            default:
                transform.localRotation = Quaternion.Lerp(transform.rotation, TempMoveVector.sqrMagnitude > 0 ? Quaternion.LookRotation(TempMoveVector) : transform.rotation, RotationSpeed * Time.deltaTime);
                break;
        }
    }
    private void FPCCameraAnimation()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
        TempMove = !InputMove.Equals(Vector2.zero);
        if (Sprint.IsPressed() && TempMove)
            CameraShake.AmplitudeGain = 1;
        else if (TempMove)
            CameraShake.AmplitudeGain = 0.25f;
        else
            CameraShake.AmplitudeGain = 0;
    }
}
