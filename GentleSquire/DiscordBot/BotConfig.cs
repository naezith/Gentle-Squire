using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;

namespace GentleSquire.DiscordBot
{
	public class BotConfig
	{
		[JsonProperty(PropertyName = "TokenPath")]
		public string TokenPath { get; private set; }

		[JsonProperty(PropertyName = "MessageQueueTimeInMilliseconds")]
		public int MessageQueueTimeInMilliseconds { get; private set; }

		[JsonProperty(PropertyName = "WorldRecordUpdatesChannelId")]
		[DefaultValue(0)]
		public ulong WorldRecordUpdatesChannelId { get; private set; }

		[JsonProperty(PropertyName = "PersonalBestUpdatesChannelId")]
		[DefaultValue(0)]
		public ulong PersonalBestUpdatesChannelId { get; private set; }

		[JsonProperty(PropertyName = "ExceptionOutputChannelId")]
		[DefaultValue(0)]
		public ulong ExceptionOutputChannelId { get; private set; }



		private BotConfig() { }



		public static BotConfig FromFile(string filepath)
		{
			return JObject.Parse(File.ReadAllText(filepath)).ToObject<BotConfig>();
		}
	}
}
