using System.IO;

namespace Dissertation.Character
{
	public enum ConversationOutput : short
	{
		None,
		TransferMoney
	}

	public class Data
	{
		public string sVal;
		public int iVal;
		public bool bVal;
		public float fVal;

		public Data()
		{
			sVal = string.Empty;
		}

		public Data(BinaryReader reader)
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

	public class ConversationFragment
	{
		public bool IsPlayer;
		public string[] ToSay;

		public ConversationOutput Output = ConversationOutput.None;
		public Data[] OptionOutputData;

		public ConversationFragment[] NextFragments;

		public ConversationFragment(bool isPlayer, int SentenceOptions)
		{
			IsPlayer = isPlayer;

			ToSay = new string[SentenceOptions];
			OptionOutputData = new Data[SentenceOptions];

			for (int idx = 0; idx < SentenceOptions; idx++)
			{
				ToSay[idx] = string.Empty;
				OptionOutputData[idx] = new Data();
			}
		}

		public ConversationFragment(BinaryReader reader)
		{
			IsPlayer = reader.ReadBoolean();

			int count = reader.ReadInt32();
			ToSay = new string[count];
			OptionOutputData = new Data[count];

			for (int idx = 0; idx < count; idx++)
			{
				ToSay[idx] = reader.ReadString();
			}

			Output = (ConversationOutput)reader.ReadInt16();

			for (int idx = 0; idx < count; idx++)
			{
				OptionOutputData[idx] = new Data(reader);
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

			foreach(Data data in OptionOutputData)
			{
				data.Serialise(writer);
			}
		}
	}
}