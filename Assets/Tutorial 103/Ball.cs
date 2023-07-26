using Fusion;

namespace Tutorial_103
{
    public class Ball : NetworkBehaviour
    {
        [Networked] private TickTimer Life { get; set; }

        public void Init()
        {
            Life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        }
    
        public override void FixedUpdateNetwork()
        {
            // 여기서 Object는 해당 컴포넌트를 가지고 있는 NetworkObject가 참조됩니다.
            if (Life.Expired(Runner))
                Runner.Despawn(Object);
            else
                transform.position += 5 * transform.forward * Runner.DeltaTime;
        
        }
    }
}
