using Dissertation.Character.Player;
using Dissertation.Util;
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
			if (agent != null)
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
			else if (_player == null)
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

			if (factionA == CharacterFaction.Player || factionB == CharacterFaction.Player)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="agent">Agent we're checking the vision on</param>
		/// <param name="hostileOnly">Don't add friendly characters to the list</param>
		/// <param name="sort">Sort by distance from agent</param>
		/// <returns>Returns list of all visible characters, ordered by distance from agent</returns>
		public List<BaseCharacterController> GetVisibleCharacters(AgentController agent, bool hostileOnly = false, bool sort = false)
		{
			List<BaseCharacterController> visible = new List<BaseCharacterController>();

			foreach (KeyValuePair<CharacterFaction, List<AgentController>> pair in _agentsByFaction)
			{
				if (hostileOnly && agent.Config.Faction == pair.Key)
				{
					continue;
				}

				foreach (AgentController other in pair.Value)
				{
					float distance = Vector3.Distance(other.transform.position, agent.transform.position);
					if (!GameObject.ReferenceEquals(other, agent) && distance <= agent._agentConfig.VisionRange)
					{
						if (!Physics2D.Raycast(agent.transform.position, agent.transform.position - other.transform.position, distance, Layers.GroundMask | Layers.DefaultMask))
						{
							visible.Add(other);
						}
					}
				}
			}

			if(!hostileOnly || IsHostileToPlayer(agent))
			{
				float distance = Vector3.Distance(_player.transform.position, agent.transform.position);
				if ( distance <= agent._agentConfig.VisionRange )
				{
					if (!Physics2D.Raycast(agent.transform.position, agent.transform.position - _player.transform.position, distance, Layers.GroundMask | Layers.DefaultMask))
					{
						visible.Add(_player);
					}
				}
			}

			if (sort)
			{
				visible.Sort(new DistanceCharacterComparer() { ComparisonPoint = agent.transform.position });
			}

			return visible;
		}

		private class DistanceCharacterComparer : IComparer<BaseCharacterController>
		{
			public Vector3 ComparisonPoint;

			public int Compare(BaseCharacterController x, BaseCharacterController y)
			{
				float xDstSqr = Vector3.SqrMagnitude(x.transform.position - ComparisonPoint);
				float yDstSqr = Vector3.SqrMagnitude(y.transform.position - ComparisonPoint);

				if(xDstSqr < yDstSqr)
				{
					return -1;
				}
				else if (xDstSqr > yDstSqr)
				{
					return 1;
				}

				return 0;
			}
		}

	}
}