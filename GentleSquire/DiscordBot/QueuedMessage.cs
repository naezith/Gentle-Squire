using DSharpPlus.Entities;

namespace GentleSquire.DiscordBot
{
	public class QueuedMessage
	{
		public DiscordChannel Channel { get; set; }
		public string Content { get; set; }



		public QueuedMessage(DiscordChannel channel, string content)
		{
			Channel = channel;
			Content = content;
		}
	}
}
