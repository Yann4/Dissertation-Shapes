using System.IO;

namespace Dissertation.Character
{
	public enum ConversationOutput : short
	{
		None,
		TransferMoney
	}

	public class ConversationFragment
	{
		public class ConversationData
		{
			public string sVal;
			public int iVal;
			public bool bVal;
			public float fVal;

			public ConversationData()
			{
				sVal = string.Empty;
			}

			public ConversationData(BinaryReader reader)
			{
				sVal = reader.ReadString();
				iVal = reader.ReadInt32();
				bVal = reader.ReadBoolean();
				fVal = reader.ReadSingle();
			}

			public void Serialise(BinaryWriter writer)
			{
				writer.Write(sVal);
				writer.Write(iVal);
				writer.Write(bVal);
				writer.Write(fVal);
			}
		}

		public bool IsPlayer;
		public string[] ToSay;

		public ConversationOutput Output = ConversationOutput.None;
		public ConversationData[] OptionOutputData;

		public ConversationFragment[] NextFragments;

		public ConversationFragment(bool isPlayer, int SentenceOptions)
		{
			IsPlayer = isPlayer;

			ToSay = new string[SentenceOptions];
			OptionOutputData = new ConversationData[SentenceOptions];

			for (int idx = 0; idx < SentenceOptions; idx++)
			{
				ToSay[idx] = string.Empty;
				OptionOutputData[idx] = new ConversationData();
			}
		}

		public ConversationFragment(BinaryReader reader)
		{
			IsPlayer = reader.ReadBoolean();

			int count = reader.ReadInt32();
			ToSay = new string[count];
			OptionOutputData = new ConversationData[count];

			for (int idx = 0; idx < count; idx++)
			{
				ToSay[idx] = reader.ReadString();
			}

			Output = (ConversationOutput)reader.ReadInt16();

			for (int idx = 0; idx < count; idx++)
			{
				OptionOutputData[idx] = new ConversationData(reader);
			}
		}

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(IsPlayer);
			writer.Write(ToSay.Length);

			foreach (string text in ToSay)
			{
				writer.Write(text);
			}

			writer.Write((short)Output);

			foreach(ConversationData data in OptionOutputData)
			{
				data.Serialise(writer);
			}
		}
	}
}