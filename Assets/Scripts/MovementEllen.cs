using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class MovementEllen : MonoBehaviour
{
    [Header("Components")]
    CharacterController controller;
    Animator animator;
    PlayerInput inputPlayer;
    InputAction moveAction;
    InputAction deathAction;
    InputAction attackAction;
    private bool canMove;
    private bool isDead;
    public CharacterController Controller { get { return controller =  controller ?? GetComponent<CharacterController>(); } }
    public Animator Animator { get { return animator = animator ?? GetComponent<Animator>(); } }
    public PlayerInput InputPlayer { get { return inputPlayer = inputPlayer ?? GetComponent<PlayerInput>(); } }
    public InputAction MoveAction => moveAction ??= InputPlayer.actions["Move"];
    public InputAction DeathAction => deathAction ??= InputPlayer.actions["Death"];
    public InputAction AttackAction => attackAction ??= InputPlayer.actions["Attack"];

    [Header("Movement Settings")]
    [SerializeField] private Transform camTransform;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float rotationSpeed = 2f;

    private float gravity = -9.81f;
    Vector3 yVector;

    void Start()
    {
        canMove = false;
        isDead = false;
    }


    void Update()
    {
        OnMove();
        Respawn();
        Death();
        OnAttack();
    }
   
    private void Death()
    {
        if (DeathAction.triggered && canMove && !isDead)
        {
            Debug.Log("Dead");
            Animator.SetTrigger("Die");
            canMove = false;
            isDead = true;

        }
    }
    private void Respawn() 
    {
        if (DeathAction.triggered && !canMove && isDead) 
        {
            Animator.SetTrigger("Respawnn");
            isDead = false;
        }
        
    }
    public void OnSpawnEnded()
    {
        Debug.Log("SpawnAnimationEnded");
        canMove = true ;
    }
    private void OnMove()
    {
        if (!canMove)
        {
            return;
        }
            var input = MoveAction.ReadValue<Vector2>();
            bool isMoving = input.magnitude > 0.1f;
            Animator.SetBool("isWalking", isMoving);
            if (isMoving)
            {
                Vector3 forwardCam = camTransform.forward;
                forwardCam.y = 0;
                forwardCam.Normalize();
                Vector3 rightCam = camTransform.right;
                rightCam.y = 0;
                rightCam.Normalize();
                var moveDirection = (forwardCam * input.y + rightCam * input.x).normalized;

                Vector3 localMove = transform.TransformDirection(new Vector3(0, 0, input.magnitude));
                Vector3 finalMove = localMove * movementSpeed;
                yVector.y += gravity * Time.deltaTime;
                Controller.Move((finalMove + yVector) * Time.deltaTime);
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

            }
        

    }

    private void OnAttack()
    {
        
        if(AttackAction.triggered && canMove && !isDead)
        {
            float[] value = { 0f, 0.5f, 1f};
            int randomIndex = Random.Range(0, value.Length);
            var randomAttack = value[randomIndex];
            Animator.SetFloat("AttackIndex",randomAttack);
            Animator.SetTrigger("Attack");

        }
        
    }
    public void MeleeAttackStart()
    {
        canMove = false;
    }
    public void MeleeAttackEnd()
    {
        canMove = true;
        Debug.Log("now u can move");
    }

}