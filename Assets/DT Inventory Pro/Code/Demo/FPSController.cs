using UnityEngine;

namespace DarkTreeFPS
{
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 1f;
        
        [HideInInspector]
        public bool lockCursor = true;
        
        [HideInInspector]
        public Rigidbody controllerRigidbody;
        
        public Transform camHolder;
        private float moveSpeedLocal;
        
        private float distanceToGround;
        
        [HideInInspector]
        public bool mouseLookEnabled = true;

        //Velocity calculation variable
        private Vector3 previousPos = new Vector3();
        
        Vector3 dirVector;

        private float defaultColliderHeight;

        public static bool canMove = true;

        [Header("MouseLook Settings")]

        private float clampY = 160;
        public Vector2 sensitivity = new Vector2(0.5f, 0.5f);
        public Vector2 smoothing = new Vector2(3, 3);
        public Vector2 startSensitivity;

        private AudioSource audioSource;

        [HideInInspector]
        public Vector2 targetDirection;
        [HideInInspector]
        public Vector2 _mouseAbsolute;
        [HideInInspector]
        public Vector2 _smoothMouse;

        public static bool canRun;

        private void OnEnable()
        {
            controllerRigidbody = GetComponent<Rigidbody>();

            defaultColliderHeight = GetComponent<CapsuleCollider>().height;

            distanceToGround = GetComponent<CapsuleCollider>().bounds.extents.y;

            audioSource = GetComponent<AudioSource>();

            startSensitivity = sensitivity;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        private void Update()
        {
            StandaloneMovement();
            
            if (Cursor.lockState != CursorLockMode.None)
                MouseLook();
        }
        
        public void FixedUpdate()
        {
            if (!canMove)
                return;

            CharacterMovement();
        }

        void MouseLook()
        {
            Quaternion targetOrientation = Quaternion.Euler(targetDirection);

            Vector2 mouseDelta = new Vector2();
            
                mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
            
            _mouseAbsolute += _smoothMouse;
            

            if (_mouseAbsolute.y < -clampY * 0.5f)
                _mouseAbsolute.y = -clampY * 0.5f;

            if (_mouseAbsolute.y > clampY * 0.5f)
                _mouseAbsolute.y = clampY * 0.5f;
            
            var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
            camHolder.transform.localRotation = xRotation;

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, camHolder.transform.InverseTransformDirection(Vector3.up));
            camHolder.transform.localRotation *= yRotation;
            camHolder.transform.rotation *= targetOrientation;
        }


        void StandaloneMovement()
        {
                if (CheckMovement())
                {
                    moveSpeedLocal = moveSpeed;
                }
        }
        
        
        void CharacterMovement()
        {

            var camForward = camHolder.transform.forward;
            var camRight = camHolder.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
                controllerRigidbody.useGravity = true;
            
                    dirVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
               
                Vector3 moveDirection = camForward * dirVector.z + camRight * dirVector.x;

                controllerRigidbody.MovePosition(transform.position + moveDirection * moveSpeedLocal * Time.deltaTime);
            
        }

        bool CheckMovementFrontal()
        {
            if (Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") == 0)
            {
                return true;
            }

            return false;
        }

        bool CheckMovement()
        {
                if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0 || Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
                {
                    return true;
                }
            
            return false;
        }
        
        public float GetVelocityMagnitude()
        {
            var velocity = ((transform.position - previousPos).magnitude) / Time.deltaTime;
            previousPos = transform.position;
            return velocity;
        }
        
        
    }
}