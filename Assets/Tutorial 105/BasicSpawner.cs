using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tutorial_105
{
    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

		private void OnGUI()
		{
			if (_runner == null)
			{
				if (GUI.Button(new Rect(0, 0, 200, 40), text: "Host"))
					StartGame(GameMode.Host);
			
				if (GUI.Button(new Rect(0, 40, 200, 40), text: "Join"))
					StartGame(GameMode.Client);
			}
		}

		async void StartGame(GameMode mode)
		{
			// NetworkRunner 컴포넌트 추가
			_runner = gameObject.AddComponent<NetworkRunner>();
			_runner.ProvideInput = true;

			await _runner.StartGame(new StartGameArgs()
			{
				GameMode = mode,
				SessionName = "TestRoom",
				Scene = SceneManager.GetActiveScene().buildIndex,
				SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
			});
		}

		[SerializeField] private NetworkPrefabRef playerPrefab;
		private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

		// 세션에 플레이어가 참가했을 경우 실행되는 함수입니다.
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			if (runner.IsServer)
			{
				// 플레이어의 캐릭터가 스폰될 위치를 정합니다.
				// 플레이어의 Id와 플레이어의 수를 이용해 X좌표를 구하는 방식입니다.
				// Photon의 튜토리얼에 포함된 코드지만, 쓸데없이 복잡한 방식인듯합니다...
				// player.RawEncoded => 플레이어 Id Index
				// runner.Config.Simulation.DefaultPlayers => 게임 세션에 기본으로 생성되는 플레이어 수
				Vector3 spawnPos = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);

				// 플레이어의 캐릭터가 될 NetworkObject 객체입니다.
				// runner.Spawn은 Fusion에서 사용되는 MonoBehaviour의 Instantiate 메소드입니다.
				NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
			
				_spawnedCharacters.Add(player, networkPlayerObject);
			}
		}

		// 플레이어가 세션에서 퇴장할 경우 호출되는 메소드입니다.
		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			// 로드한 플레이어의 캐릭터를 삭제합니다.
			if (_spawnedCharacters.TryGetValue(player, out var networkObject))
			{
				runner.Despawn(networkObject);
				_spawnedCharacters.Remove(player);
			}
		}


		private bool _mouseButton0;
		private bool _mouseButton1;
		private void Update()
		{
			_mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
			_mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
		}

		// Fusion은 정확한 동기화와 입력 정보를 관리하기 위해 NetworkInput을 이용합니다.
		public void OnInput(NetworkRunner runner, NetworkInput input)
		{
			// NetworkInput을 사용하기 위해서는 INetworkInput 인터페이스가 구현된 구조체가 필요합니다.
			// NetworkInputData는 직접 작성된 INetWorkInput이 구현된 구조체입니다.
			var data = new NetworkInputData();

			if (Input.GetKey(KeyCode.W))
				data.Direction += Vector3.forward;
		
			if (Input.GetKey(KeyCode.S))
				data.Direction += Vector3.back;
		
			if (Input.GetKey(KeyCode.A))
				data.Direction += Vector3.left;
		
			if (Input.GetKey(KeyCode.D))
				data.Direction += Vector3.right;

			// _mouseButton0이 true일 경우, data.Buttons에 NetworkInputData.MOUSE_BUTTON1 값을 추가합니다.
			if (_mouseButton0)
				data.Buttons |= NetworkInputData.MOUSE_BUTTON1;
			if (_mouseButton1)
				data.Buttons |= NetworkInputData.MOUSE_BUTTON2;

			_mouseButton0 = false;
			_mouseButton1 = false;
		
			input.Set(data);
		}

		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
		public void OnConnectedToServer(NetworkRunner runner) { }
		public void OnDisconnectedFromServer(NetworkRunner runner) { }
		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnSceneLoadStart(NetworkRunner runner) { }
	}
}