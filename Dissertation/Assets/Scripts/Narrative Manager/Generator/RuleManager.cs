using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Dissertation.Narrative.Generator
{
	public class RuleManager
	{
		public struct Rule
		{
			public Token Left;
			public List<Token> Right;

			//Matches the format "Rulename($(tag:1), $(tag:1))" - values within the parenthesis is optional, and handles multiple or zero params
			private static Regex m_RuleRegex = new Regex(@"(?:([A-z]+)\s*(?:\((?:\$\(([A-z]+):([0-9]+)\),*\s*)+\)?)?)", RegexOptions.Compiled);
			public Rule(string ruleToParse, BeatTemplates templates)
			{
				Right = new List<Token>();
				Left = new Token();
				string[] ruleSides = ruleToParse.Split('>');

				Match lhsMatch = m_RuleRegex.Match(ruleSides[0]);
				List<Parameter> parameters = new List<Parameter>();
				for(int groupIdx = 2; groupIdx < lhsMatch.Groups.Count;)
				{
					string tag = lhsMatch.Groups[groupIdx++].Value;
					string value = lhsMatch.Groups[groupIdx++].Value;

					if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(value))
					{
						parameters.Add(new Parameter(tag, value));
					}
				}

				string ruleName = lhsMatch.Groups[1].Value;
				TextAsset graph = templates.GetGraph(ruleName);

				Left = graph == null ? new Token(ruleName, parameters.ToArray())
					: new Token(ruleName, graph, parameters.ToArray());

				foreach (Match match in m_RuleRegex.Matches(ruleSides[1]))
				{
					parameters.Clear();

					for (int groupIdx = 2; groupIdx < match.Groups.Count;)
					{
						string tag = match.Groups[groupIdx++].Value;
						string value = match.Groups[groupIdx++].Value;

						if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(value))
						{
							parameters.Add(new Parameter(tag, value));
						}
					}

					ruleName = match.Groups[1].Value;
					graph = templates.GetGraph(ruleName);
					Right.Add(graph == null ? new Token(ruleName, parameters.ToArray())
					: new Token(ruleName, graph, parameters.ToArray()));
				}
			}
		}

		List<Rule> m_Rules = new List<Rule>();
		BeatTemplates m_Templates;

		public RuleManager(TextAsset ruleSet, BeatTemplates templates)
		{
			m_Templates = templates;

			foreach(string line in ruleSet.text.Split('\n'))
			{
				m_Rules.Add(new Rule(line, templates));
			}
		}
	}
}