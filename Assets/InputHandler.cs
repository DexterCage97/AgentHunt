using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputHandler : MonoBehaviour
{
    [Header("Input Action Asset")] [SerializeField]
    private InputActionAsset playerControls;

    [Header("Action Map Name Refrences")] [SerializeField]
    private string actionMapName = "Player";

    #region ActionNameReferences

    [Header("Action Name References")] [SerializeField]
    private string movement = "Movement";

    [SerializeField] private string mouseMovement = "MouseMovement";
    [SerializeField] private string rollLeft = "RollLeft";
    [SerializeField] private string rollRight = "RollRight";
    [SerializeField] private string rollForward = "RollForward";
    [SerializeField] private string rollBackward = "RollBackward";
    [SerializeField] private string reload = "Reload";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string takecover = "TakeCover";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string cancel = "Cancel";
    [SerializeField] private string grenade = "Grenade";
    [SerializeField] private string melee = "Melee";
    [SerializeField] private string zoom = "Zoom";
    [SerializeField] private string block = "Block";
    [SerializeField] private string scope = "Scope";
    [SerializeField] private string climb = "Climb";
    [SerializeField] private string run = "Run";
    [SerializeField] private string updateWeapon0 = "UpdateWeapon0";
    [SerializeField] private string updateWeapon1 = "UpdateWeapon1";
    [SerializeField] private string updateWeapon2 = "UpdateWeapon2";
    [SerializeField] private string updateWeapon3 = "UpdateWeapon3";
    [SerializeField] private string updateWeapon4 = "UpdateWeapon4";
    [SerializeField] private string updateWeapon5 = "UpdateWeapon5";
    [SerializeField] private string updateWeapon6 = "UpdateWeapon6";
    [SerializeField] private string updateWeapon7 = "UpdateWeapon7";
    [SerializeField] private string updateWeapon8 = "UpdateWeapon8";
    [SerializeField] private string updateWeapon9 = "UpdateWeapon9";

    [SerializeField] private string mouseScrollDelta = "MouseScrollDelta";

    #endregion

    #region InputActions

    private InputAction movementAction;
    private InputAction mouseMovementAction;

    private InputAction rollLeftAction;
    private InputAction rollRightAction;
    private InputAction rollForwardAction;
    private InputAction rollBackwardAction;

    private InputAction reloadAction;
    private InputAction jumpAction;
    private InputAction takecoverAction;
    private InputAction crouchAction;
    private InputAction fireAction;
    private InputAction cancelAction;
    private InputAction grenadeAction;
    private InputAction meleeAction;
    private InputAction zoomAction;
    private InputAction blockAction;
    private InputAction scopeAction;
    private InputAction climbAction;
    private InputAction runAction;

    private InputAction updateWeapon0Action;
    private InputAction updateWeapon1Action;
    private InputAction updateWeapon2Action;
    private InputAction updateWeapon3Action;
    private InputAction updateWeapon4Action;
    private InputAction updateWeapon5Action;
    private InputAction updateWeapon6Action;
    private InputAction updateWeapon7Action;
    private InputAction updateWeapon8Action;
    private InputAction updateWeapon9Action;

    private InputAction mouseScrollDeltaAction;

    #endregion


    #region InputTypes

    public Vector2 movementInput { get; private set; }
    public Vector2 mouseMovementInput { get; private set; }

    public bool mouseScrollDeltaInput { get; private set; }

    public bool rollLeftInput { get; private set; }
    public bool rollRightInput { get; private set; }
    public bool rollForwardInput { get; private set; }
    public bool rollBackwardInput { get; private set; }

    public bool reloadInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool takecoverInput { get; private set; }
    public bool crouchInput { get; private set; }
    public bool fireInput { get; private set; }
    public bool cancelInput { get; private set; }
    public bool grenadeInput { get; private set; }
    public bool meleeInput { get; private set; }
    public bool zoomInput { get; private set; }
    public bool blockInput { get; private set; }
    public bool scopeInput { get; private set; }
    public bool climbInput { get; private set; }
    public bool runInput { get; private set; }

    public bool updateWeapon0Input { get; private set; }
    public bool updateWeapon1Input { get; private set; }
    public bool updateWeapon2Input { get; private set; }
    public bool updateWeapon3Input { get; private set; }
    public bool updateWeapon4Input { get; private set; }
    public bool updateWeapon5Input { get; private set; }
    public bool updateWeapon6Input { get; private set; }
    public bool updateWeapon7Input { get; private set; }
    public bool updateWeapon8Input { get; private set; }
    public bool updateWeapon9Input { get; private set; }

    #endregion

    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;


        movementAction = playerControls.FindActionMap(actionMapName).FindAction(movement);
        mouseMovementAction = playerControls.FindActionMap(actionMapName).FindAction(mouseMovement);
        mouseScrollDeltaAction = playerControls.FindActionMap(actionMapName).FindAction(mouseScrollDelta);

        rollLeftAction = playerControls.FindActionMap(actionMapName).FindAction(rollLeft);
        rollRightAction = playerControls.FindActionMap(actionMapName).FindAction(rollRight);
        rollForwardAction = playerControls.FindActionMap(actionMapName).FindAction(rollForward);
        rollBackwardAction = playerControls.FindActionMap(actionMapName).FindAction(rollBackward);

        reloadAction = playerControls.FindActionMap(actionMapName).FindAction(reload);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        takecoverAction = playerControls.FindActionMap(actionMapName).FindAction(takecover);
        crouchAction = playerControls.FindActionMap(actionMapName).FindAction(crouch);
        fireAction = playerControls.FindActionMap(actionMapName).FindAction(fire);
        cancelAction = playerControls.FindActionMap(actionMapName).FindAction(cancel);
        grenadeAction = playerControls.FindActionMap(actionMapName).FindAction(grenade);
        meleeAction = playerControls.FindActionMap(actionMapName).FindAction(melee);
        zoomAction = playerControls.FindActionMap(actionMapName).FindAction(zoom);
        blockAction = playerControls.FindActionMap(actionMapName).FindAction(block);
        scopeAction = playerControls.FindActionMap(actionMapName).FindAction(scope);
        climbAction = playerControls.FindActionMap(actionMapName).FindAction(climb);
        runAction = playerControls.FindActionMap(actionMapName).FindAction(run);

        updateWeapon0Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon0);
        updateWeapon1Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon1);
        updateWeapon2Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon2);
        updateWeapon3Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon3);
        updateWeapon4Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon4);
        updateWeapon5Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon5);
        updateWeapon6Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon6);
        updateWeapon7Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon7);
        updateWeapon8Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon8);
        updateWeapon9Action = playerControls.FindActionMap(actionMapName).FindAction(updateWeapon9);


        RegisterInputActions();
    }

    private void OnEnable()
    {
        movementAction.Enable();
        mouseMovementAction.Enable();
        mouseScrollDeltaAction.Enable();

        rollLeftAction.Enable();
        rollRightAction.Enable();
        rollForwardAction.Enable();
        rollBackwardAction.Enable();

        reloadAction.Enable();
        jumpAction.Enable();
        takecoverAction.Enable();
        crouchAction.Enable();
        fireAction.Enable();
        cancelAction.Enable();
        grenadeAction.Enable();
        meleeAction.Enable();
        zoomAction.Enable();
        blockAction.Enable();
        scopeAction.Enable();
        climbAction.Enable();
        runAction.Enable();

        updateWeapon0Action.Enable();
        updateWeapon1Action.Enable();
        updateWeapon2Action.Enable();
        updateWeapon3Action.Enable();
        updateWeapon4Action.Enable();
        updateWeapon5Action.Enable();
        updateWeapon6Action.Enable();
        updateWeapon7Action.Enable();
        updateWeapon8Action.Enable();
        updateWeapon9Action.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
        mouseMovementAction.Disable();
        mouseScrollDeltaAction.Disable();

        rollLeftAction.Disable();
        rollRightAction.Disable();
        rollForwardAction.Disable();
        rollBackwardAction.Disable();

        reloadAction.Disable();
        jumpAction.Disable();
        takecoverAction.Disable();
        crouchAction.Disable();
        fireAction.Disable();
        cancelAction.Disable();
        grenadeAction.Disable();
        meleeAction.Disable();
        zoomAction.Disable();
        blockAction.Disable();
        scopeAction.Disable();
        climbAction.Disable();
        runAction.Disable();

        updateWeapon0Action.Disable();
        updateWeapon1Action.Disable();
        updateWeapon2Action.Disable();
        updateWeapon3Action.Disable();
        updateWeapon4Action.Disable();
        updateWeapon5Action.Disable();
        updateWeapon6Action.Disable();
        updateWeapon7Action.Disable();
        updateWeapon8Action.Disable();
        updateWeapon9Action.Disable();
    }

    void RegisterInputActions()
    {
        movementAction.performed += context => movementInput = context.ReadValue<Vector2>();
        movementAction.canceled += context => movementInput = Vector2.zero;

        mouseMovementAction.performed += context => mouseMovementInput = context.ReadValue<Vector2>();
        mouseMovementAction.canceled += context => mouseMovementInput = Vector2.zero;

        mouseScrollDeltaAction.performed += context => mouseScrollDeltaInput = true;
        mouseScrollDeltaAction.canceled += context => mouseScrollDeltaInput = false;

        rollLeftAction.performed += context => rollLeftInput = true;
        rollLeftAction.canceled += context => rollLeftInput = false;

        rollRightAction.performed += context => rollRightInput = true;
        rollRightAction.canceled += context => rollRightInput = false;

        rollForwardAction.performed += context => rollForwardInput = true;
        rollForwardAction.canceled += context => rollForwardInput = false;

        rollBackwardAction.performed += context => rollBackwardInput = true;
        rollBackwardAction.canceled += context => rollBackwardInput = false;


        reloadAction.performed += context => reloadInput = true;
        reloadAction.canceled += context => reloadInput = false;

        jumpAction.performed += context => jumpInput = true;
        jumpAction.canceled += context => jumpInput = false;

        takecoverAction.performed += context => takecoverInput = true;
        takecoverAction.canceled += context => takecoverInput = false;

        crouchAction.performed += context => crouchInput = true;
        crouchAction.canceled += context => crouchInput = false;

        fireAction.performed += context => fireInput = true;
        fireAction.canceled += context => fireInput = false;

        cancelAction.performed += context => cancelInput = true;
        cancelAction.canceled += context => cancelInput = false;

        grenadeAction.performed += context => grenadeInput = true;
        grenadeAction.canceled += context => grenadeInput = false;

        meleeAction.performed += context => meleeInput = true;
        meleeAction.canceled += context => meleeInput = false;

        zoomAction.performed += context => zoomInput = true;
        zoomAction.canceled += context => zoomInput = false;

        blockAction.performed += context => blockInput = true;
        blockAction.canceled += context => blockInput = false;

        scopeAction.performed += context => scopeInput = true;
        scopeAction.canceled += context => scopeInput = false;

        climbAction.performed += context => climbInput = true;
        climbAction.canceled += context => climbInput = false;

        runAction.performed += context => runInput = true;
        runAction.canceled += context => runInput = false;


        updateWeapon0Action.performed += context => updateWeapon0Input = true;
        updateWeapon0Action.canceled += context => updateWeapon0Input = false;

        updateWeapon1Action.performed += context => updateWeapon1Input = true;
        updateWeapon1Action.canceled += context => updateWeapon1Input = false;

        updateWeapon2Action.performed += context => updateWeapon2Input = true;
        updateWeapon2Action.canceled += context => updateWeapon2Input = false;

        updateWeapon3Action.performed += context => updateWeapon3Input = true;
        updateWeapon3Action.canceled += context => updateWeapon3Input = false;

        updateWeapon4Action.performed += context => updateWeapon4Input = true;
        updateWeapon4Action.canceled += context => updateWeapon4Input = false;

        updateWeapon5Action.performed += context => updateWeapon5Input = true;
        updateWeapon5Action.canceled += context => updateWeapon5Input = false;

        updateWeapon6Action.performed += context => updateWeapon6Input = true;
        updateWeapon6Action.canceled += context => updateWeapon6Input = false;

        updateWeapon7Action.performed += context => updateWeapon7Input = true;
        updateWeapon7Action.canceled += context => updateWeapon7Input = false;

        updateWeapon8Action.performed += context => updateWeapon8Input = true;
        updateWeapon8Action.canceled += context => updateWeapon8Input = false;

        updateWeapon9Action.performed += context => updateWeapon9Input = true;
        updateWeapon9Action.canceled += context => updateWeapon9Input = false;
    }
}