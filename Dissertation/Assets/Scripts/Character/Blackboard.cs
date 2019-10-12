using Dissertation.Character.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class Blackboard
	{
		private PlayerController _player;
		private Dictionary<CharacterFaction, List<AgentController>> _agentsByFaction = new Dictionary<CharacterFaction, List<AgentController>>();
		private Dictionary<AgentController, bool> _hostileToPlayer = new Dictionary<AgentController, bool>();

		public Blackboard()
		{
			Spawner.OnSpawn += OnCharacterSpawned;

			_agentsByFaction[CharacterFaction.Triangle] = new List<AgentController>();
			_agentsByFaction[CharacterFaction.Circle] = new List<AgentController>();
			_agentsByFaction[CharacterFaction.Square] = new List<AgentController>();
		}

		~Blackboard()
		{
			Spawner.OnSpawn -= OnCharacterSpawned;
		}

		private void OnCharacterSpawned(BaseCharacterController character)
		{
			AgentController agent = character as AgentController;
			if(agent != null)
			{
				CharacterFaction faction = agent.Config.Faction;
				if (!_agentsByFaction[faction].Contains(agent))
				{
					_agentsByFaction[faction].Add(agent);
				}

				if (!_hostileToPlayer.ContainsKey(agent))
				{
					_hostileToPlayer[agent] = false;
				}
			}
			else if(_player == null)
			{
				//Must be a player
				_player = character as PlayerController;
				Debug.Assert(_player != null);
			}
		}

		public bool IsHostileToPlayer(AgentController agent)
		{
			return _hostileToPlayer[agent];
		}

		public bool CharactersAreHostile(BaseCharacterController characterA, BaseCharacterController characterB)
		{
			CharacterFaction factionA = characterA.Config.Faction;
			CharacterFaction factionB = characterB.Config.Faction;

			if(factionA == CharacterFaction.Player || factionB == CharacterFaction.Player)
			{
				AgentController agent = factionA == CharacterFaction.Player ? characterB as AgentController : characterA as AgentController;
				return _hostileToPlayer[agent];
			}

			//All factions are hostile to one another
			return factionA != factionB;
		}

		public void MarkAsHostileToPlayer(AgentController agent)
		{
			_hostileToPlayer[agent] = true;
		}
	}
}