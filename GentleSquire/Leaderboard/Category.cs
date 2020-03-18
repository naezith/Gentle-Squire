using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GentleSquire.Leaderboard
{
	public class Category
	{
		public enum CategoryType { IndividualLevel, Speedrun }

		[JsonConverter(typeof(StringEnumConverter))]
		public CategoryType Type { get; set; }

		public int Id { get; set; }
		public string Name { get; set; }
	}
}
