using System.Collections.Generic;
using UnityEngine;
using Weighted_Randomizer;
using System;
using System.Linq;

namespace Dissertation.Narrative.Generator
{
	public class RuleManager
	{
		private const string STORYLINE = "Storyline";
		private List<Rule> _Rules = new List<Rule>();
		private BeatTemplates _Templates;

		public RuleManager(TextAsset ruleSet, BeatTemplates templates)
		{
			_Templates = templates;

			foreach(string line in ruleSet.text.Split('\n'))
			{
				_Rules.Add(new Rule(line, templates));
			}
		}

		public IEnumerable<Rule> GetStorylineRules()
		{
			return _Rules.Where(rule => rule.Left.Name == STORYLINE);
		}

		public Rule GetWeightedRandomMatchingRule(Token token, HashSet<NarrativeObject> existingNarrativeObjects)
		{
			float totalScore = 0;
			List<Tuple<Rule, float, NarrativeObject[]>> matches = new List<Tuple<Rule, float, NarrativeObject[]>>();
			foreach (Rule rule in _Rules.Where(r => r.Left.Name == token.Name))
			{ 
				if (WorldStateMatches(rule, existingNarrativeObjects, out NarrativeObject[] ruleObjects, out int reusedObjects))
				{
					float score = 1f +							//1 point for existing
						rule.Left.Parameters.Length +			//More parameters == more specific rule == more good
						ParameterListMatchScore(token, rule) +  //If the parameter lists match, they're more likely to be a good fit
						(reusedObjects * 10f);					//Reusing narrative objects very good

					matches.Add(new Tuple<Rule, float, NarrativeObject[]>(rule, score, ruleObjects));
					totalScore += score;
				}
			}

			DynamicWeightedRandomizer<Rule> weightedRules = new DynamicWeightedRandomizer<Rule>();
			foreach (Tuple<Rule, float, NarrativeObject[]> match in matches)
			{
				weightedRules.Add(match.Item1, (int)((match.Item2 / totalScore) * 100.0f));
			}

			Rule selected = weightedRules.NextWithRemoval();
			existingNarrativeObjects.UnionWith(matches.Find(match => match.Item1 == selected).Item3);
			return selected;
		}

		private bool WorldStateMatches(Rule rule, HashSet<NarrativeObject> existingNarrativeObjects, out NarrativeObject[] ruleObjects, out int reusedObjects)
		{
			if(rule.Left.Parameters.Length == 0)
			{
				ruleObjects = new NarrativeObject[0];
				reusedObjects = 0;
				return true;
			}

			ruleObjects = GetNarrativeObjectForRule(rule, existingNarrativeObjects, out reusedObjects); ;

			return ruleObjects != null;
		}

		private NarrativeObject[] GetNarrativeObjectForRule(Rule rule, HashSet<NarrativeObject> existingNarrativeObjects, out int numReused)
		{
			List < NarrativeObject > parms = new List<NarrativeObject>();

			numReused = 0;
			//In this implementation, possibility for the same object to be reused for multiple parameters
			foreach (Parameter parameter in rule.Left.Parameters)
			{
				NarrativeObject obj = NarrativeObject.GetObjectAND(parameter.Tag, out bool reused, existingNarrativeObjects);
				if(obj == null)
				{
					return null;
				}

				numReused += reused ? 1 : 0;
				parms.Add(obj);
			}

			return parms.ToArray();
		}

		private float ParameterListMatchScore(Token lhs, Rule rhs)
		{
			if(lhs.Parameters.Length != rhs.Left.Parameters.Length)
			{
				return 0f;
			}

			for(int idx = 0; idx < lhs.Parameters.Length; idx++)
			{
				if(!lhs.Parameters[idx].Equals(rhs.Left.Parameters[idx]))
				{
					return 0f;
				}
			}

			return 30f;
		}
	}
}