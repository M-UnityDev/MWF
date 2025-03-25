using Unity.Cinemachine;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public CharacterController cntrl;
    public float baseSpd;
    public float jumpHght;
    public float sprintSpd;
    public float Rot;
    [SerializeField] private bool pause;
    [SerializeField] private GameObject PausePan;
    [SerializeField] private AudioSource Footstep;
    [SerializeField] private AudioClip JumpSound;
    [SerializeField] private CinemachineCamera Camera;
    [SerializeField] private GameObject Cam;
    [SerializeField] private float spdBst;
    [SerializeField] private float x;
    [SerializeField] private float z;
    private Vector3 vel;
    void Start() => Cursor.lockState = CursorLockMode.Locked;
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        Rot = Cam.transform.eulerAngles.y;
        gameObject.GetComponent<Transform>().eulerAngles = new Vector3(gameObject.GetComponent<Transform>().eulerAngles.x, Rot, gameObject.GetComponent<Transform>().eulerAngles.z);
        if (Input.GetButtonDown("Cancel"))
            Puse();
        if (Input.GetButton("Jump") && cntrl.isGrounded)
        {
            vel.y = Mathf.Sqrt(jumpHght * -2 * Physics.gravity.y);
            gameObject.GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }
        if (cntrl.isGrounded && vel.y < 0)
            vel.y = -2;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            spdBst = sprintSpd;
            Footstep.pitch = 2;
        }
        else
        {
            spdBst = 0;
            Footstep.pitch = 1;
        }
        Vector3 move = transform.right * x + transform.forward * z;
        cntrl.Move((baseSpd + spdBst) * Time.deltaTime * move);
        vel.y += Physics.gravity.y * Time.deltaTime;
        cntrl.Move(vel * Time.deltaTime);
        if (x != 0 || Input.GetAxisRaw("Vertical") != 0)
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0.25f;
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0 || Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") != 0)
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
        else if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            Camera.GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
        if (cntrl.isGrounded && Input.GetAxisRaw("Horizontal") != 0 || cntrl.isGrounded && Input.GetAxisRaw("Vertical") != 0)
            Footstep.mute = false;
        else
            Footstep.mute = true;
    }
    public void Puse()
    {
        pause = !pause;
        Time.timeScale = pause ? 0 : 1;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pause;
        PausePan.SetActive(pause);
    }
}