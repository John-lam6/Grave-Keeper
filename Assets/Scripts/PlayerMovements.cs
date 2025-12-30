using UnityEngine;

public class PlayerMovements : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private Animator m_Animator;
    private Vector3 m_Movement;

    public Rigidbody m_Rigidbody;
    public float m_Speed = 10f;
    private bool canMove = false;
    
    private float movementX;
    private float movementY;
    
    [Header("Raycast")]
    [SerializeField] private Camera camera;
    

    void Start() {
        m_Animator = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody> ();
    }


    void Update() {
        if (canMove) {
            // receive inputs for movement and rotation (mouse)
            movementX = Input.GetAxisRaw("Horizontal");
            movementY = Input.GetAxisRaw("Vertical");

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                Vector3 lookDir = hit.point - transform.position;
                lookDir.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookDir);
                transform.rotation = rotation;
            }
        }
    }
    
    void FixedUpdate() {
        Vector3 moveDir = new Vector3(movementX, 0, movementY).normalized;
        
        Vector3 newVelocity = moveDir * m_Speed;
        newVelocity.y = m_Rigidbody.velocity.y; 
        
        // checks if player is moving, if it is, animate running
        bool hasHorizontalInput = !Mathf.Approximately(movementX, 0f);
        bool hasVerticalInput = !Mathf.Approximately(movementY, 0f);
        bool isRunning = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("isRunning", isRunning);

        m_Rigidbody.velocity = newVelocity;
    }
    
    private void OnAnimatorMove() {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
    }
    
    public void setCanMove (bool canMove) {
        this.canMove = canMove;    
    }
}
