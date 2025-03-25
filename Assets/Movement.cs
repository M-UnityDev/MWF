using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem; 
public class Movement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction Move;
    [SerializeField] private InputAction Interact;
    [SerializeField] private InputAction Sprint;
    [SerializeField] private InputAction Jump;
    [Header("Parameters")]
    [SerializeField] private int BaseSpeed;
    [SerializeField] private int SprintSpeed;
    [SerializeField] private int JumpHeight;
    [SerializeField] private bool IsFPC;
    [Header("GameObjects")]
    [SerializeField] private CinemachineCamera Camera;
    [SerializeField] private Transform CameraTransform;
    private CharacterController CharacterControl;
    private int SpeedBoost;
    private Vector3 Velocity;
    private Vector2 TempMove;
    private void Awake()
    {
        
        CharacterControl = GetComponent<CharacterController>();
    }
    private void Update()
    {
        UpdateJump();
        UpdateSprint();
        UpdatePhysics();
        if (IsFPC)
            FPCCameraAnimation();
        //UpdateSound();
    }
    private void FPCCameraAnimation()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, CameraTransform.eulerAngles.y, transform.eulerAngles.z);
        TempMove = Move.ReadValue<Vector2>();
        if (!TempMove.Equals(Vector2.zero))
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0.25f;
        else if (Sprint.WasReleasedThisFrame() && !TempMove.Equals(Vector2.zero))
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
        else if (TempMove.Equals(Vector2.zero))
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
    }
    private void UpdateSprint()
    {
        if (Sprint.WasReleasedThisFrame())
        {
            SpeedBoost = SprintSpeed;
            //Footstep.pitch = 2;
        }
        else
        {
            SpeedBoost = 0;
            //Footstep.pitch = 1;
        }
    }
    private void UpdateJump()
    {
        if (Jump.WasReleasedThisFrame() && CharacterControl.isGrounded)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Physics.gravity.y);
            //GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }
        if (CharacterControl.isGrounded && Velocity.y < 0)
            Velocity.y = -2;
    }
    private void UpdateSound()
    {
        //if (CharacterControl.isGrounded && Move.ReadValue<Vector2>() != Vector2.zero)
            //Footstep.mute = false;
        //else
            //Footstep.mute = true;
    }
    private void UpdatePhysics()
    {
        CharacterControl.Move((BaseSpeed + SpeedBoost) * Time.deltaTime * Move.ReadValue<Vector2>());
        Velocity.y += Physics.gravity.y * Time.deltaTime;
        CharacterControl.Move(Velocity * Time.deltaTime);
    }
}
