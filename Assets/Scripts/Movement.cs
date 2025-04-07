using System.Collections;
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
    [SerializeField] private AudioClip SecondPhaseMusic;
    [SerializeField] private AudioClip DeathSound;
    [SerializeField] private LayerMask LayersToInclude;
    [SerializeField] private LayerMask Walls;
    public int DistanceToWalkFuckYou { get => DistanceToWalkSqr; set => DistanceToWalkSqr = value; }
    [SerializeField] private int DistanceToCheck;
    [SerializeField] private bool IsFPC;
    [Header("GameObjects")]
    [SerializeField] private Material[] SecondPlayerMaterial;
    [SerializeField] private Material SecondPlayerFPCMaterial;
    [SerializeField] private Material SkyMaterial;
    [SerializeField] private AudioSource Footstep;
    public Transform AnotherPlayer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin CameraShake;
    [SerializeField] private CinemachineCamera AbsoluteCinema;
    [SerializeField] private GameObject FPC;
    [SerializeField] private GameObject FPCSkin;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private CharacterController CharacterControl;
    [SerializeField] private Material TempMaterial;
    [Header("Animation")]
    [SerializeField] private GameObject Model;
    [SerializeField] private GameObject Head;
    [SerializeField] private GameObject Body;
    [SerializeField] private GameObject ArmLeft;
    [SerializeField] private GameObject ArmRight;
    [SerializeField] private GameObject RightLeg;
    [SerializeField] private GameObject LeftLeg;
    [Header("Debug Values")]
    private int CurrentSpeed;
    private float HorizontalAxis;
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
        SkyMaterial.color = Color.green;
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
        TempMoveVector = PlayerPrefs.GetInt("Invert",0).Equals(0) ? new Vector3(InputMove.x, 0, InputMove.y) : new Vector3(InputMove.y, 0, -InputMove.x);
    }
    private void FixedUpdate() => UpdatePhysics();
    private void Update()
    {
        HorizontalAxis = Mathf.Lerp(HorizontalAxis, InputMove.x,5*Time.deltaTime);
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
        if (CharacterControl.enabled) CharacterControl.Move(Time.deltaTime * ((CurrentSpeed * MoveVector) + Velocity));
    }
    private void UpdateCollisons()
    {
        Colliders = Physics.OverlapBox(transform.position + transform.forward * DistanceToCheck, Vector3.one / 2, transform.localRotation, LayersToInclude);
        foreach (Collider Item in Colliders)
        {
            if (Item.CompareTag("Finish"))
            {
                StartCoroutine(WinCutScene(Item.gameObject));
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
        Footstep.pitch = IsRunning && IsFPC ? 1 : 0.75f;
        if (IsFPC) Footstep.panStereo = Random.Range(-1f,1f);
    }
    private void UpdateRotation()
    {
        if(IsFPC)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
            AbsoluteCinema.Lens.Dutch = -HorizontalAxis*5;
            TempMove = !InputMove.Equals(Vector2.zero);
            switch (IsRunning, TempMove)
            {
                case (true,true):
                    CameraShake.AmplitudeGain = 0.5f;
                    Vibrator.StartVibrate(20,20);
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
    private void UpdateOutline()
    {
        if (IsFPC)
            outline.SetActive(Physics.RaycastAll(transform.position, AnotherPlayer.position - transform.position, Mathf.Sqrt(Vector3.SqrMagnitude(AnotherPlayer.position - transform.position)), Walls).Length > 0);
    }
    private IEnumerator WinCutScene(GameObject Item)
    {
        Item.SetActive(false);
        GetComponent<AudioSource>().PlayOneShot(SecondPhaseMusic);
        transform.DORotate(Vector3.up*180,1);
        Camera.main.transform.parent.GetComponent<CinemachineTargetGroup>().RemoveMember(AnotherPlayer);
        UpdateJump();
        yield return new WaitForSeconds(0.45f);
        CharacterControl.enabled = false;
        FPC.SetActive(true);
        Model.SetActive(true);
        LeftLeg.SetActive(true);
        RightLeg.SetActive(true);
        Model.transform.DOLocalMoveY(-1.28f,1).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(0.9f);
        Body.SetActive(true);
        ArmLeft.SetActive(true);
        ArmRight.SetActive(true);
        Model.transform.DOLocalMoveY(-2,1);
        yield return new WaitForSeconds(0.9f);
        ArmLeft.transform.DOLocalMoveX(-0.3125f,1).SetEase(Ease.InOutCubic);
        ArmRight.transform.DOLocalMoveX(0.3125f,1).SetEase(Ease.InOutCubic);
        //yield return new WaitForSeconds(1);
        Head.SetActive(true);
        Head.transform.localScale = Vector3.one*2;
        GetComponent<MeshRenderer>().enabled = false;
        Head.transform.DOScale(Vector3.one,1);
        yield return new WaitForSeconds(1);
        Model.transform.DOLocalMoveY(-0.5f,1);
        transform.DORotate(Vector3.zero,1);
        yield return new WaitForSeconds(0.5f);
        if (Velocity.y < 0) Velocity.y = -2;
        CharacterControl.enabled = true;
        CameraTransform.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Destroy(Camera.main.transform.parent.GetComponent<CinemachineCamera>());
        Destroy(Camera.main.transform.parent.GetComponent<CinemachineGroupFraming>());
        Destroy(Camera.main.transform.parent.GetComponent<CinemachineTargetGroup>());
        Destroy(Camera.main.transform.parent.GetComponent<CinemachineDeoccluder>());
        Destroy(Camera.main.transform.parent.GetComponent<CameraTriggerDirector>());
        IsFPC = true;
        AnotherPlayer.GetComponent<Movement>().DistanceToWalkFuckYou = int.MaxValue;
        SkyMaterial.DOColor(Color.red,1);
        TempMaterial.color = new Color(0.5f,1,0.5f);
        TempMaterial.DOColor(new Color(1,0.5f,0.5f),1).OnUpdate(() => {RenderSettings.ambientLight = TempMaterial.color;});
        Camera.main.transform.parent = null;
        GameObject.Find("OutlineCamera").transform.parent = null;
        //FPCamera.SetActive(true);
        //FPCameraOutline.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;

    }
}
