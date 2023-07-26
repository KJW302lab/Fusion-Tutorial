using Fusion;
using Tutorial_102;
using UnityEngine;

namespace Tutorial_103
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Ball prefabBall;
    
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
                    }   
                }
            }
        }
    }   
}
