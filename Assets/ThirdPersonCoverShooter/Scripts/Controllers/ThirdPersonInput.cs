using System;
using Dreamteck.Splines;
using UnityEngine;

namespace CoverShooter
{
    /// <summary>
    /// Describes a key used to trigger a custom action, animation and message names.
    /// </summary>
    [Serializable]
    public struct CustomAction
    {
        /// <summary>
        /// Key to be pressed to active the trigger.
        /// </summary>
        [Tooltip("Key to be pressed to active the trigger.")]
        public KeyCode Key;

        /// <summary>
        /// Name of the animation trigger.
        /// </summary>
        [Tooltip("Name of the animation trigger.")]
        public string Name;

        /// <summary>
        /// Name of the message.
        /// </summary>
        [Tooltip("Name of the message.")] public string Message;
    }

    /// <summary>
    /// Takes player input and transform it to commands to ThirdPersonController.
    /// </summary>
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(ThirdPersonController))]
    public class ThirdPersonInput : MonoBehaviour
    {
        /// <summary>
        /// Camera moved by this input component.
        /// </summary>
        public ThirdPersonCamera Camera
        {
            get
            {
                if (CameraOverride != null)
                    return CameraOverride;
                else
                {
                    if (CameraManager.Main != _cachedCameraOwner)
                    {
                        _cachedCameraOwner = CameraManager.Main;

                        if (_cachedCameraOwner == null)
                            _cachedCamera = null;
                        else
                            _cachedCamera = _cachedCameraOwner.GetComponent<ThirdPersonCamera>();
                    }

                    return _cachedCamera;
                }
            }
        }

        /// <summary>
        /// Always turn towards the movement direction when not aiming.
        /// </summary>
        [Tooltip("Always turn towards the movement direction when not aiming.")]
        public bool DirectionalMovement = true;

        /// <summary>
        /// Is character running instead of walking when moving.
        /// </summary>
        [Tooltip("Is character running instead of walking when moving.")]
        public bool FastMovement = true;

        /// <summary>
        /// Is character slowing to to a walk when zooming in.
        /// </summary>
        [Tooltip("Is character slowing to to a walk when zooming in.")]
        public bool WalkWhenZooming = true;

        /// <summary>
        /// Camera to rotate around the player. If set to none it is taken from the main camera.
        /// </summary>
        [Tooltip("Camera to rotate around the player. If set to none it is taken from the main camera.")]
        public ThirdPersonCamera CameraOverride;

        /// <summary>
        /// Multiplier for horizontal camera rotation.
        /// </summary>
        [Tooltip("Multiplier for horizontal camera rotation.")] [Range(0, 10)]
        public float HorizontalRotateSpeed = 2.0f;

        /// <summary>
        /// Multiplier for vertical camera rotation.
        /// </summary>
        [Tooltip("Multiplier for vertical camera rotation.")] [Range(0, 10)]
        public float VerticalRotateSpeed = 1.0f;

        /// <summary>
        /// Multiplier to rotation speeds when zooming in. Speed is already adjusted by the FOV difference.
        /// </summary>
        [Tooltip("Multiplier to rotation speeds when zooming in. Speed is already adjusted by the FOV difference.")]
        [Range(0, 10)]
        public float ZoomRotateMultiplier = 1.0f;

        /// <summary>
        /// Is camera responding to mouse movement when the mouse cursor is unlocked.
        /// </summary>
        [Tooltip("Is camera responding to mouse movement when the mouse cursor is unlocked.")]
        public bool RotateWhenUnlocked = false;

        /// <summary>
        /// Maximum time in seconds to wait for a second tap to active rolling.
        /// </summary>
        [Tooltip("Maximum time in seconds to wait for a second tap to active rolling.")]
        public float DoubleTapDelay = 0.3f;

        /// <summary>
        /// Keys to be pressed to activate custom actions.
        /// </summary>
        [Tooltip("Keys to be pressed to activate custom actions.")]
        public CustomAction[] CustomActions;

        /// <summary>
        /// Input is ignored when a disabler is active.
        /// </summary>
        [Tooltip("Input is ignored when a disabler is active.")]
        public GameObject Disabler;

        private CharacterMotor _motor;
        private ThirdPersonController _controller;
        private CharacterInventory _inventory;

        private Camera _cachedCameraOwner;
        private ThirdPersonCamera _cachedCamera;

        private float _timeW;
        private float _timeA;
        private float _timeS;
        private float _timeD;

        private float _leftMoveIntensity = 1;
        private float _rightMoveIntensity = 1;
        private float _backMoveIntensity = 1;
        private float _frontMoveIntensity = 1;

        private SplineProjector splineprojector;

        private void Awake()
        {
            splineprojector = GetComponent<SplineProjector>();
            _controller = GetComponent<ThirdPersonController>();
            _motor = GetComponent<CharacterMotor>();
            _inventory = GetComponent<CharacterInventory>();

            _controller.WaitForUpdateCall = true;
        }

        private void OnEnable()
        {
            splineprojector.onEndReached += splineEndReached;
        }

        private void OnDisable()
        {
            splineprojector.onEndReached -= splineEndReached;
        }

        private void splineEndReached()
        {
            StartMovement = false;
            splineprojector.enabled = false;
            Camera.IsInCover = true;
            inputWeapon(currentWeapon+1);
          //  _controller.ZoomInput = true;
            //_controller.ZoomInput = false;
        }

        private void Update()
        {
            if (Disabler != null && Disabler.activeSelf)
                return;

            UpdateCamera();
            UpdateTarget();
            UpdateCustomActions();
            UpdateMovement();
            UpdateWeapons();
            UpdateReload();
            UpdateRolling();
            UpdateAttack();
            UpdateGrenade();
            UpdateCrouching();
            UpdateClimbing();
            UpdateCover();
            UpdateJumping();

            _controller.ManualUpdate();
        }

        protected virtual void UpdateCustomActions()
        {
            foreach (var action in CustomActions)
                if (Input.GetKeyDown(action.Key))
                {
                    if (action.Message != null && action.Message.Length > 0)
                        SendMessage(action.Message, SendMessageOptions.RequireReceiver);

                    if (action.Name != null && action.Name.Length > 0)
                        SendMessage("OnCustomAction", action.Name, SendMessageOptions.RequireReceiver);
                }
        }


        [SerializeField] private bool StartMovement;

        protected virtual void UpdateMovement()
        {
            // var local = InputHandler.Instance.movementInput.x * Vector3.right +
            //          InputHandler.Instance.movementInput.y * Vector3.forward;

            Vector3 local = StartMovement ? Vector3.forward : Vector3.zero;

//            print(local);
            var movement = new CharacterMovement();
            movement.Direction = getMovementDirection(local);

            if (WalkWhenZooming && _controller.ZoomInput)
            {
                movement.Magnitude = 0.5f;
                movement.IsSlowedDown = true;
            }
            else
            {
                if ((_motor.ActiveWeapon.Gun != null || _motor.ActiveWeapon.HasMelee) && FastMovement)
                {
                    if (InputHandler.Instance.runInput && !_motor.IsCrouching)
                        movement.Magnitude = 2.0f;
                    else
                        movement.Magnitude = 1.0f;
                }
                else
                {
                    if (InputHandler.Instance.runInput)
                        movement.Magnitude = 1.0f;
                    else
                        movement.Magnitude = 0.5f;
                }
            }

            _controller.MovementInput = movement;
        }

        protected virtual void UpdateClimbing()
        {
            if (InputHandler.Instance.climbInput)
            {
                var direction = InputHandler.Instance.movementInput.x * Vector3.right +
                                InputHandler.Instance.movementInput.y * Vector3.forward;

                if (direction.magnitude > float.Epsilon)
                {
                    direction = Quaternion.Euler(0, aimAngle, 0) * direction.normalized;

                    var cover = _motor.GetClimbableInDirection(direction);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                }
            }
        }

        protected virtual void UpdateCover()
        {
            if (InputHandler.Instance.takecoverInput)
                _controller.InputTakeCover();
        }

        protected virtual void UpdateJumping()
        {
            if (InputHandler.Instance.jumpInput)
            {
                var direction = InputHandler.Instance.movementInput.x * Vector3.right +
                                InputHandler.Instance.movementInput.y * Vector3.forward;

                if (direction.magnitude > float.Epsilon)
                    _controller.InputJump(Util.HorizontalAngle(direction) + aimAngle);
                else
                    _controller.InputJump();
            }
        }

        protected virtual void UpdateCrouching()
        {
            if (InputHandler.Instance.crouchInput)
                _controller.InputCrouch();
        }

        protected virtual void UpdateGrenade()
        {
            if (_motor.HasGrenadeInHand)
            {
                if (InputHandler.Instance.fireInput)
                    _controller.InputThrowGrenade();

                if (InputHandler.Instance.cancelInput)
                    _controller.InputCancelGrenade();
            }

            if (InputHandler.Instance.grenadeInput)
                _controller.InputTakeGrenade();
        }

        protected virtual void UpdateAttack()
        {
            _controller.FireInput = InputHandler.Instance.fireInput;
            _controller.ZoomInput = InputHandler.Instance.zoomInput;
            _controller.BlockInput = InputHandler.Instance.blockInput;

            if (InputHandler.Instance.meleeInput)
                _controller.InputMelee();

            if (_controller.IsZooming)
            {
                if (InputHandler.Instance.scopeInput)
                    _controller.ScopeInput = !_controller.ScopeInput;
            }
            else
                _controller.ScopeInput = false;
        }

        protected virtual void UpdateRolling()
        {
            if (_timeW > 0) _timeW -= Time.deltaTime;
            if (_timeA > 0) _timeA -= Time.deltaTime;
            if (_timeS > 0) _timeS -= Time.deltaTime;
            if (_timeD > 0) _timeD -= Time.deltaTime;

            if (InputHandler.Instance.rollForwardInput)
            {
                if (_timeW > float.Epsilon)
                {
                    var cover = _motor.GetClimbambleInDirection(aimAngle);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                    else
                        roll(Vector3.forward);
                }
                else
                    _timeW = DoubleTapDelay;
            }

            if (InputHandler.Instance.rollLeftInput)
            {
                if (_timeA > float.Epsilon)
                {
                    var cover = _motor.GetClimbambleInDirection(aimAngle - 90);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                    else
                        roll(-Vector3.right);
                }
                else
                    _timeA = DoubleTapDelay;
            }

            if (InputHandler.Instance.rollBackwardInput)
            {
                if (_timeS > float.Epsilon)
                {
                    var cover = _motor.GetClimbambleInDirection(aimAngle + 180);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                    else
                        roll(-Vector3.forward);
                }
                else
                    _timeS = DoubleTapDelay;
            }

            if (InputHandler.Instance.rollRightInput)
            {
                if (_timeD > float.Epsilon)
                {
                    var cover = _motor.GetClimbambleInDirection(aimAngle + 90);

                    if (cover != null)
                        _controller.InputClimbOrVault(cover);
                    else
                        roll(Vector3.right);
                }
                else
                    _timeD = DoubleTapDelay;
            }
        }

        protected virtual void UpdateWeapons()
        {
            if (InputHandler.Instance.updateWeapon1Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(0);
            }

            if (InputHandler.Instance.updateWeapon2Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(1);
            }

            if (InputHandler.Instance.updateWeapon3Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(2);
            }

            if (InputHandler.Instance.updateWeapon4Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(3);
            }

            if (InputHandler.Instance.updateWeapon5Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(4);
            }

            if (InputHandler.Instance.updateWeapon6Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(5);
            }

            if (InputHandler.Instance.updateWeapon7Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(6);
            }

            if (InputHandler.Instance.updateWeapon8Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(7);
            }

            if (InputHandler.Instance.updateWeapon9Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(8);
            }

            if (InputHandler.Instance.updateWeapon0Input)
            {
                _motor.InputCancelGrenade();
                inputWeapon(9);
            }

            if (InputHandler.Instance.mouseScrollDeltaInput.y < 0)
            {
                if (currentWeapon == 0 && _inventory != null)
                    inputWeapon(_inventory.Weapons.Length);
                else
                    inputWeapon(currentWeapon - 1);
            }
            else if (InputHandler.Instance.mouseScrollDeltaInput.y > 0)
            {
                if (_inventory != null && currentWeapon == _inventory.Weapons.Length)
                { inputWeapon(0);}
                else
                { inputWeapon(currentWeapon + 1);}
            }
        }

        private int currentWeapon
        {
            get
            {
                if (_inventory == null || !_motor.IsEquipped)
                    return 0;

                for (int i = 0; i < _inventory.Weapons.Length; i++)
                    if (_inventory.Weapons[i].IsTheSame(ref _motor.Weapon))
                        return i + 1;

                return 0;
            }
        }

        private void inputWeapon(int index)
        {
            if (_inventory == null && index > 0)
                return;

            if (index <= 0 || (_inventory != null && index > _inventory.Weapons.Length))
                _controller.InputUnequip();
            else if (_inventory != null && index <= _inventory.Weapons.Length)
                _controller.InputEquip(_inventory.Weapons[index - 1]);
        }

        protected virtual void UpdateReload()
        {
            if (InputHandler.Instance.reloadInput)
                _controller.InputReload();
        }

        protected virtual void UpdateTarget()
        {
            if (_controller == null)
                return;

            var camera = Camera;
            if (camera == null) return;

            var inaccurateTarget = camera.CalculateAimTarget(false);
            var accurateTarget = Util.GetClosestStaticHit(camera.transform.position, inaccurateTarget, 0);

            if (_motor.isFiringFromCamera && _motor.ActiveWeapon.Gun != null)
            {
                var preciseTarget = camera.CalculateAimTarget(true);
                var preciseHit = Util.GetClosestStaticHit(camera.transform.position, preciseTarget, 0);

                _motor.ActiveWeapon.Gun.SetupRaycastThisFrame(camera.transform.position, preciseHit);
            }

            if (DirectionalMovement && !_motor.IsAiming && !_controller.ZoomInput && !_controller.FireInput)
            {
                var direction = InputHandler.Instance.movementInput.x * camera.transform.right +
                                InputHandler.Instance.movementInput.y * camera.transform.forward;

                _controller.BodyTargetInput = _motor.transform.position + direction * 16;
            }
            else
                _controller.BodyTargetInput = inaccurateTarget;

            _controller.AimTargetInput = accurateTarget;
            _controller.GrenadeHorizontalAngleInput = Util.HorizontalAngle(camera.transform.forward);
            _controller.GrenadeVerticalAngleInput = Mathf.Asin(camera.transform.forward.y) * 180f / Mathf.PI;
        }

        protected virtual void UpdateCamera()
        {
            var camera = Camera;
            if (camera == null) return;


            if (Cursor.lockState == CursorLockMode.Locked || RotateWhenUnlocked)
            {
                var scale = 1.0f;

                if (_motor != null)
                {
                    var gun = _motor.ActiveWeapon.Gun;

                    if (_controller.IsScoping && gun != null)
                        scale = ZoomRotateMultiplier * (1.0f - gun.Zoom / camera.StateFOV);
                    else if (_controller.IsZooming)
                        scale = ZoomRotateMultiplier * (1.0f - camera.Zoom / camera.StateFOV);
                }


                float horVal = 0;
                float verVal = 0;

                if (StartMovement)
                {
                    horVal = InputHandler.Instance.mouseMovementInput.x * HorizontalRotateSpeed * scale;
                    // verVal = InputHandler.Instance.mouseMovementInput.y * VerticalRotateSpeed * scale;
                    //  horVal = 0;
                    verVal = 0;
                }
                else
                {
                    horVal = InputHandler.Instance.mouseMovementInput.x * HorizontalRotateSpeed * scale;
                    verVal = InputHandler.Instance.mouseMovementInput.y * VerticalRotateSpeed * scale;
                }

//print(horVal);
                camera.Horizontal += horVal;
                camera.Vertical -= verVal;
                camera.UpdatePosition();
            }


            _motor.InputVerticalMeleeAngle(camera.Vertical);
        }

        private void roll(Vector3 local)
        {
            var direction = getMovementDirection(local);

            if (direction.sqrMagnitude > float.Epsilon)
                _controller.InputRoll(Util.HorizontalAngle(direction));
        }

        private Vector3 getMovementDirection(Vector3 local)
        {
            var forward = Camera == null ? _motor.transform.forward : Camera.transform.forward;
            var right = Vector3.Cross(Vector3.up, forward);

            float angle = Util.HorizontalAngle(forward);

            var check_right = right;
            var check_forward = forward;

            if (_motor.IsInCover)
            {
                _leftMoveIntensity = 0;
                _rightMoveIntensity = 0;
                _frontMoveIntensity = 0;
                _backMoveIntensity = 0;
            }
            else
            {
                Util.Lerp(ref _leftMoveIntensity, _motor.IsFreeToMove(-check_right) ? 1.0f : 0.0f, 4);
                Util.Lerp(ref _rightMoveIntensity, _motor.IsFreeToMove(check_right) ? 1.0f : 0.0f, 4);
                Util.Lerp(ref _backMoveIntensity, _motor.IsFreeToMove(-check_forward) ? 1.0f : 0.0f, 4);
                Util.Lerp(ref _frontMoveIntensity, _motor.IsFreeToMove(check_forward) ? 1.0f : 0.0f, 4);

                if (local.x < -float.Epsilon) local.x *= _leftMoveIntensity;
                if (local.x > float.Epsilon) local.x *= _rightMoveIntensity;
                if (local.z < -float.Epsilon) local.z *= _backMoveIntensity;
                if (local.z > float.Epsilon) local.z *= _frontMoveIntensity;
            }

            return Quaternion.Euler(0, angle, 0) * local;
        }

        private float aimAngle
        {
            get { return Util.HorizontalAngle(_controller.AimTargetInput - transform.position); }
        }
    }
}