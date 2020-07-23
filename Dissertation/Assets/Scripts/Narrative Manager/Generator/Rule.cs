using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	[Serializable]
	public class Rule : IComparable<Rule>
	{
		public Token Left;
		public List<Token> Right;
		public bool Decomposable { get { return Right.Any(token => token.CanDecompose); } }

		//Matches the format "Rulename($(tag:1), $(tag:1))" - values within the parenthesis is optional, and handles multiple or zero params
		private static Regex m_RuleRegex = new Regex(@"(?:([A-z]+)\s*(?:\((?:\$\(([A-z]+):([0-9]+)\),*\s*)+\)?)?)", RegexOptions.Compiled);

		public Rule(string ruleToParse, BeatTemplates templates)
		{
			Right = new List<Token>();
			Left = new Token();
			string[] ruleSides = ruleToParse.Split('>');

			Match lhsMatch = m_RuleRegex.Match(ruleSides[0]);
			List<Parameter> parameters = new List<Parameter>();
			for (int groupIdx = 2; groupIdx < lhsMatch.Groups.Count;) //Start at 2 because 0 is the complete match & 1 is the name
			{
				string tag = lhsMatch.Groups[groupIdx++].Value;
				string value = lhsMatch.Groups[groupIdx++].Value;

				if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(value))
				{
					parameters.Add(new Parameter(tag, value));
				}
			}

			string ruleName = lhsMatch.Groups[1].Value;
			TextAsset graph = templates.GetTemplate(ruleName);

			Left = graph == null ? new Token(ruleName, parameters.ToArray())
				: new Token(ruleName, graph, parameters.ToArray());

			foreach (Match match in m_RuleRegex.Matches(ruleSides[1]))
			{
				parameters.Clear();

				for (int groupIdx = 2; groupIdx < match.Groups.Count;) //Start at 2 because 0 is the complete match & 1 is the name
				{
					string tag = match.Groups[groupIdx++].Value;
					string value = match.Groups[groupIdx++].Value;

					if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(value))
					{
						parameters.Add(new Parameter(tag, value));
					}
				}

				ruleName = match.Groups[1].Value;
				graph = templates.GetTemplate(ruleName);
				Right.Add(graph == null ? new Token(ruleName, parameters.ToArray())
				: new Token(ruleName, graph, parameters.ToArray()));
			}

			Debug.Assert(Right.Count >= 1, "Must have values on the right hand side of the rule");
		}

		public Rule(Rule other)
		{
			Left = other.Left;
			Right = new List<Token>(other.Right.Capacity);
			foreach(Token rhs in other.Right)
			{
				Right.Add(rhs);
			}
		}

		public int CompareTo(Rule other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return 1;
			}

			int result = Left.CompareTo(other.Left);
			if (result == 0)
			{
				if (Right.Count == other.Right.Count)
				{
					for (int idx = 0; idx < Right.Count; idx++)
					{
						result = Right[idx].CompareTo(other.Right[idx]);
						if (result != 0)
						{
							break;
						}
					}
				}
				else
				{
					result = Right.Count > other.Right.Count ? 1 : -1;
				}
			}

			return result;
		}
	}
}