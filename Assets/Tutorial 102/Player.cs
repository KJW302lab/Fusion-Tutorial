using Fusion;

public class Player : NetworkBehaviour
{
    // 플레이어 프리팹이 가진 플레이어 컨트롤러입니다.
    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
            _cc.Move(5 * data.Direction.normalized * Runner.DeltaTime);
    }
}
