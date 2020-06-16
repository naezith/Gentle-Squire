using GentleSquire.DiscordBot;

namespace GentleSquire
{
	class Program
	{
		public static void Main()
		{
			new Bot().RunAsync().GetAwaiter().GetResult();
		}
	}
}
