using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField] private LayerMask LayersToInclude;
    [SerializeField] private LayerMask Walls;
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
    private VibrationDirector Vibrator;
    private GameObject outline;
    private Collider[] Colliders;
    private Vector2 InputMove;
    private void Awake()
    {
        FindFirstObjectByType<CinemachineTargetGroup>().AddMember(transform, 10, 5);
        TempAnotherPlayers = FindObjectsByType<Movement>(FindObjectsSortMode.None);
        Vibrator = FindFirstObjectByType<VibrationDirector>();
        foreach (Movement Player in TempAnotherPlayers)
        {
            if(!Player.gameObject.Equals(gameObject))
            {
                CharacterControl.enabled = false;
                transform.position = new Vector3(0,0.5f,2);
                CharacterControl.enabled = true;
                AnotherPlayer = Player.transform;
                outline = AnotherPlayer.Find("Outline").gameObject;
                AnotherPlayer.GetComponent<Movement>().outline = transform.Find("Outline").gameObject;
                Player.AnotherPlayer = transform;
                GetComponent<MeshRenderer>().materials = SecondPlayerMaterial;
                foreach (Transform parts in FPCSkin.transform)
                    foreach (MeshRenderer ms in parts.GetComponentsInChildren<MeshRenderer>(true))
                        ms.material = SecondPlayerFPCMaterial;
                return;
            }
            else if(AnotherPlayer.Equals(null)) AnotherPlayer = transform;
        }
    }
    public void OnDisconnect() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void OnJump(CallbackContext context)
    {
        if(context.performed) UpdateJump();
    }
    public void OnSprint(CallbackContext context) => IsRunning = context.performed;
    public void OnMove(CallbackContext context)
    {
        InputMove = context.ReadValue<Vector2>();
        TempMoveVector = PlayerPrefs.GetInt("Invert",0).Equals(0) ? new Vector3(InputMove.x, 0, InputMove.y) : new Vector3(InputMove.y, 0, InputMove.x);
    }
    private void FixedUpdate() => UpdatePhysics();
    private void Update()
    {
        UpdateSpeedChanges();
        UpdateCollisons();
        UpdateSound();
        UpdateRotation();
        UpdateOutline();
    }
    private void UpdateJump()
    {
        if (CharacterControl.isGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
            GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }
        if (CharacterControl.isGrounded && Velocity.y < 0) Velocity.y = -2;
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
        CharacterControl.Move(Time.deltaTime * ((CurrentSpeed * MoveVector) + Velocity));
    }
    private void UpdateCollisons()
    {
        Colliders = Physics.OverlapBox(transform.position + transform.forward * DistanceToCheck, Vector3.one / 2, transform.localRotation, LayersToInclude);
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
                Item.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<ParticleSystem>().Play();
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
        Footstep.mute = !CharacterControl.isGrounded || InputMove.Equals(Vector2.zero) || !IsFPC;
        Footstep.pitch = IsRunning && IsFPC ? 2 : 1;
    }
    private void UpdateRotation()
    {
        if(IsFPC)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
            TempMove = !InputMove.Equals(Vector2.zero);
            switch (IsRunning, TempMove)
            {
                case (true,true):
                    CameraShake.AmplitudeGain = 1;
                    Vibrator.StartVibrate(1,1);
                break;
                case (false,true):
                    CameraShake.AmplitudeGain = 0.25f;
                    Vibrator.StartVibrate(0.1f,0.1f);
                break;
                default:
                    CameraShake.AmplitudeGain = 0;
                    Vibrator.StopVibrate();
                break;
            }
        }
        else transform.localRotation = Quaternion.Lerp(transform.rotation, TempMoveVector.sqrMagnitude > 0 ? Quaternion.LookRotation(TempMoveVector) : transform.rotation, RotationSpeed * Time.deltaTime);
    }
    public void UpdateOutline()
    {
        if (IsFPC)
            outline.SetActive(Physics.RaycastAll(transform.position, AnotherPlayer.position - transform.position, Mathf.Sqrt(Vector3.SqrMagnitude(AnotherPlayer.position - transform.position)), Walls).Length > 0);
    }
}
