namespace Assets.Scripts.Game.Characters.Core.Player.Input
{
	public struct ButtonState
	{
		public bool Held;
		public bool PressedThisFrame;
		public bool ReleasedThisFrame;

		public void ClearEdges()
		{
			PressedThisFrame = false;
			ReleasedThisFrame = false;
		}
	}
}
