using Fusion;
using UnityEngine;

namespace Tutorial_105
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Ball prefabBall;
        [SerializeField] private PhysXBall prefabPhysXBall;

        private Vector3 _forward;

        [Networked] private TickTimer Delay { get; set; }
    
        // 플레이어 프리팹이 가진 플레이어 컨트롤러입니다.
        private NetworkCharacterControllerPrototype _cc;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterControllerPrototype>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.Direction.Normalize();
                _cc.Move(5 * data.Direction * Runner.DeltaTime);

                if (data.Direction.sqrMagnitude > 0)
                    _forward = data.Direction;

                if (Delay.ExpiredOrNotRunning(Runner))
                {
                    if ((data.Buttons & NetworkInputData.MOUSE_BUTTON1) != 0)
                    {
                        Delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    
                        Runner.Spawn(
                            prefab: prefabBall,
                            position: transform.position + _forward,
                            rotation: Quaternion.LookRotation(_forward),
                            inputAuthority: Object.InputAuthority,
                            // Ball이 생성되기 전에 초기화를 진행합니다.
                            onBeforeSpawned: (runner, obj) => obj.GetComponent<Ball>().Init());

                        Spawned = !Spawned;
                    }
                    
                    else if ((data.Buttons & NetworkInputData.MOUSE_BUTTON2) != 0)
                    {
                        Delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        
                        Runner.Spawn(
                            prefab: prefabPhysXBall,
                            position: transform.position + _forward,
                            rotation: Quaternion.LookRotation(_forward),
                            inputAuthority: Object.InputAuthority,
                            // PhysXBall이 생성되기 전에 초기화를 진행합니다.
                            onBeforeSpawned: (runner, obj) => obj.GetComponent<PhysXBall>().Init(10 * _forward));
                        
                        Spawned = !Spawned;
                    }
                }
            }
        }
        
        [Networked(OnChanged = nameof(OnBallSpawned))] public NetworkBool Spawned { get; set; }

        public static void OnBallSpawned(Changed<Player> changed)
        {
            changed.Behaviour.Material.color = Color.white;
        }
        
        private Material _material;

        public Material Material
        {
            get
            {
                if(_material == null)
                    _material = GetComponentInChildren<MeshRenderer>().material;
                return _material;
            }
        }
        
        public override void Render()
        {
            // Ball이 생성될 당시에 흰색이었다가, 점차 파란색으로 바뀝니다.
            Material.color = Color.Lerp(Material.color, Color.blue, Time.deltaTime );
        }
    }   
}