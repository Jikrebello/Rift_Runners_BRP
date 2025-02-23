using System.Collections.Generic;

namespace Assets.Scripts.Player
{
	public interface IPlayerState
	{
		PlayerContext PlayerContext { get; set; }
		void Enter(Dictionary<string, object> parameters);
		void Update();
		void FixedUpdate();
		void Exit();
	}
}
