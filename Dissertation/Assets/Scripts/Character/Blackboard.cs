using Dissertation.Character.Player;
using Dissertation.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class Blackboard
	{
		public PlayerController Player { get; private set; }
		private Dictionary<CharacterFaction, List<AgentController>> _agentsByFaction = new Dictionary<CharacterFaction, List<AgentController>>();
		private Dictionary<AgentController, bool> _hostileToPlayer = new Dictionary<AgentController, bool>();

		public List<BaseCharacterController> Criminals = new List<BaseCharacterController>();

		private ConversationData _conversations;

		public bool PlayerIsInConversation { get { return _inConversationWithPlayer != null; } }
		private AgentController _inConversationWithPlayer = null;

		private List<Inventory> _minePoints = new List<Inventory>();

		public Blackboard( ConversationData conversations )
		{
			Spawner.OnSpawn += OnCharacterSpawned;
			Conversation.ConversationStarted += OnConverationStarted;
			Conversation.ConversationEnded += OnConversationEnded;

			_agentsByFaction[CharacterFaction.Triangle] = new List<AgentController>();
			_agentsByFaction[CharacterFaction.Circle] = new List<AgentController>();
			_agentsByFaction[CharacterFaction.Square] = new List<AgentController>();

			_conversations = conversations;
			_conversations.Setup();

			App.OnLevelLoaded += OnLevelLoaded;
		}

		~Blackboard()
		{
			Spawner.OnSpawn -= OnCharacterSpawned;
			Conversation.ConversationStarted -= OnConverationStarted;
			Conversation.ConversationEnded -= OnConversationEnded;

			App.OnLevelLoaded -= OnLevelLoaded;
		}

		private void OnLevelLoaded()
		{
			_minePoints = GameObject.FindObjectsOfType<Inventory>().Where(inventory => inventory.OnGround && inventory.Owner == null).ToList();
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
			else if (Player == null)
			{
				//Must be a player
				Player = character as PlayerController;
				Debug.Assert(Player != null);
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
			agent.AddEnemy(CharacterFaction.Player);
			Player.AddEnemy(agent.Config.Faction);
			_hostileToPlayer[agent] = true;
		}

		public void EndHostilityToPlayer(AgentController agent)
		{
			agent.RemoveEnemy(CharacterFaction.Player);
			Player.RemoveEnemy(agent.Config.Faction);
			_hostileToPlayer[agent] = false;
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
					if (!GameObject.ReferenceEquals(other, agent) && CanSeeCharacter(agent, other))
					{
						visible.Add(other);
					}
				}
			}

			if((!hostileOnly || IsHostileToPlayer(agent)) && CanSeeCharacter(agent, Player))
			{
				visible.Add(Player);
			}

			if (sort)
			{
				visible.Sort(new DistanceCharacterComparer() { ComparisonPoint = agent.transform.position });
			}

			return visible;
		}

		public bool CanSeeCharacter(AgentController agent, BaseCharacterController other)
		{
			float distance = Vector3.Distance(other.transform.position, agent.transform.position);

			return LineOfSightCheck(agent.transform, other.transform, agent._agentConfig.VisionRange);
		}

		public bool LineOfSightCheck(Transform a, Transform b, float visionRange)
		{
			float distance = Vector3.Distance(a.position, b.position);
			return distance <= visionRange &&
				!Physics2D.Raycast(a.position, (b.position - a.position).normalized,
									distance, Layers.GroundMask | Layers.DefaultMask);
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

		public void AddCriminal(BaseCharacterController criminal)
		{
			if (!Criminals.Contains(criminal))
			{
				criminal.Health.OnDied += OnCriminalDie;
				Criminals.Add(criminal);
			}
		}

		private void OnCriminalDie(BaseCharacterController died)
		{
			Criminals.Remove(died);
			died.Health.OnDied -= OnCriminalDie;
		}

		public ConversationFragment GetConversation(string conversationReference)
		{
			return _conversations.GetConversation(conversationReference);
		}

		public List<Inventory> GetAvailableMinePoints()
		{
			return _minePoints.Where(inventory => !inventory.Contents.IsEmpty()).ToList();
		}

		private void OnConverationStarted(ConversationFragment conversation, AgentController owner)
		{
			if(_conversations.IsPlayerConversation(conversation))
			{
				_inConversationWithPlayer = owner;
			}
		}

		private void OnConversationEnded(AgentController owner)
		{
			if(owner == _inConversationWithPlayer)
			{
				_inConversationWithPlayer = null;
			}
		}
	}
}
