using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEngine.InputSystem.InputAction;
public class Movement : MonoBehaviour
{ 
    [Header("Parameters")]
    [SerializeField] private int BaseSpeed;
    [SerializeField] private int SprintSpeed;
    [SerializeField] private int SlowSpeed;
    [SerializeField] private int JumpHeight;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private int DistanceToWalkSqr;
    [SerializeField] private AudioClip JumpSound;
    [SerializeField] private AudioClip DeathSound;
    public int DistanceToWalkFuckYou { get => DistanceToWalkSqr; set => DistanceToWalkSqr = value; }
    [SerializeField] private int DistanceToCheck;
    [SerializeField] private bool IsFPC;
    [Header("GameObjects")]
    [SerializeField] private Material[] SecondPlayerMaterial;
    [SerializeField] private Material SecondPlayerFPCMaterial;
    [SerializeField] private AudioSource Footstep;
    public Transform AnotherPlayer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin CameraShake;
    [SerializeField] private GameObject FPC;
    [SerializeField] private GameObject FPCSkin;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private CharacterController CharacterControl;
    [Header("Debug Values")]
    private int CurrentSpeed;
    private Vector3 Velocity;
    private Vector3 MoveVector = Vector3.zero;
    private Vector3 TempMoveVector;
    private bool TempMove;
    private Movement[] TempAnotherPlayers;
    private bool IsClose;
    private bool IsRunning;
    private bool IsWin;
    private Collider[] Colliders;
    private Vector2 InputMove;
    private void Awake()
    {
        FindFirstObjectByType<CinemachineTargetGroup>().AddMember(transform, 10, 5);
        TempAnotherPlayers = FindObjectsByType<Movement>(FindObjectsSortMode.None);
        
        foreach (Movement Player in TempAnotherPlayers)
        {
            switch (!Player.gameObject.Equals(gameObject))
            {
                case true:
                    CharacterControl.enabled = false;
                    transform.position = new Vector3(2,0.5f);
                    CharacterControl.enabled = true;
                    AnotherPlayer = Player.transform;
                    Player.AnotherPlayer = transform;
                    GetComponent<MeshRenderer>().materials = SecondPlayerMaterial;
                    foreach (Transform parts in FPCSkin.transform)
                        foreach (MeshRenderer ms in parts.GetComponentsInChildren<MeshRenderer>(true))
                            ms.material = SecondPlayerFPCMaterial;
                    return;
                case false:
                    if(AnotherPlayer.Equals(null))
                        AnotherPlayer = transform;
                    break;
            }
        }
    }
    private void Update()
    {
        UpdateSpeedChanges();
        UpdatePhysics();
        UpdateCollisons();
        UpdateSound();
        UpdateRotation();
    }
    public void OnJump(CallbackContext context)
    {
        if(context.performed)
            UpdateJump();
    }
    public void OnSprint(CallbackContext context)
    {
        IsRunning = context.performed;
    }
    public void OnMove(CallbackContext context)
    {
        InputMove = context.ReadValue<Vector2>();
        TempMoveVector = new Vector3(InputMove.x, 0, InputMove.y);
    }
    private void UpdateJump()
    {
        if (CharacterControl.isGrounded)
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
        if (IsRunning && IsClose)
            CurrentSpeed = SprintSpeed;
        else if (IsClose)
            CurrentSpeed = BaseSpeed;
        else
            CurrentSpeed = SlowSpeed;
    }
    private void UpdatePhysics()
    {
        MoveVector = IsFPC ? transform.right * TempMoveVector.x + transform.forward * TempMoveVector.z : TempMoveVector;
        Velocity.y += Physics.gravity.y * Time.deltaTime;
        CharacterControl.Move(CurrentSpeed * Time.deltaTime * (MoveVector + Velocity));
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
                
                Destroy(Camera.main.transform.parent.GetComponent<CinemachineCamera>());
                Destroy(Camera.main.transform.parent.GetComponent<CinemachineGroupFraming>());
                Destroy(Camera.main.transform.parent.GetComponent<CinemachineDecollider>());
                Destroy(Camera.main.transform.parent.GetComponent<CameraTriggerDirector>());
                Camera.main.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            else if (Item.transform.Equals(AnotherPlayer) && IsFPC && !IsWin)
            {
                IsWin = true;
                Destroy(Item.GetComponent<Movement>());
                Item.GetComponent<AudioSource>().PlayOneShot(DeathSound);
                Item.GetComponentInChildren<SpriteRenderer>().enabled = true;
                Item.GetComponentInChildren<SpriteRenderer>().gameObject.GetComponentInChildren<ParticleSystem>().Play();
                Item.GetComponentInChildren<SpriteRenderer>().DOFade(1,0.5f);
                Destroy(Item.GetComponent<MeshRenderer>());
                Destroy(Item.GetComponent<PlayerInput>());
                Destroy(Item.GetComponent<CharacterController>());
                Destroy(Item.GetComponent<BoxCollider>());
                return;
            }
        }
    }
    private void UpdateSound()
    {
        if (IsFPC)
        {
            Footstep.mute = !CharacterControl.isGrounded || InputMove.Equals(Vector2.zero);
            Footstep.pitch = IsRunning ? 2 : 1;
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
        if (IsRunning && TempMove)
            CameraShake.AmplitudeGain = 1;
        else if (TempMove)
            CameraShake.AmplitudeGain = 0.25f;
        else
            CameraShake.AmplitudeGain = 0;
    }
}
