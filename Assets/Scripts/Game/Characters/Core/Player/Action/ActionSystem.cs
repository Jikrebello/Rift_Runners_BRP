using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action
{
	public sealed class ActionSystem
	{
		// MVP: just fire triggers when allowed.
		// Later: add action locking, stamina, buffering, cancel windows.
		public void HandleIntents(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is LightAttackIntent)
				{
					outputs.Animation.AddTrigger(AnimTrigger.LightAttack);
					continue;
				}

				if (intent is HeavyAttackIntent)
				{
					outputs.Animation.AddTrigger(AnimTrigger.HeavyAttack);
					continue;
				}

				if (intent is ContextInteractIntent)
				{
					outputs.Animation.AddTrigger(AnimTrigger.ContextInteract);
					continue;
				}

				if (intent is ContextGrabOrFireIntent)
				{
					outputs.Animation.AddTrigger(AnimTrigger.ContextGrabOrFire);
					continue;
				}

				if (intent is UseSkillIntent skill)
				{
					switch (skill.Slot)
					{
						case 1:
							outputs.Animation.AddTrigger(AnimTrigger.Skill1);
							break;

						case 2:
							outputs.Animation.AddTrigger(AnimTrigger.Skill2);
							break;

						case 3:
							outputs.Animation.AddTrigger(AnimTrigger.Skill3);
							break;
					}
				}
			}
		}
	}
}
