using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace GentleSquire.Leaderboard
{
	public class Category : IEquatable<Category>
	{
		public enum CategoryType { IndividualLevel, Speedrun }



		public static List<Category> Categories { get; private set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public CategoryType Type { get; set; }

		public int Id { get; set; }
		public string Name { get; set; }



		public static void SetCategoryList(string filepath)
		{
			Categories = JArray.Parse(File.ReadAllText(filepath)).ToObject<List<Category>>();
		}

		public static Category GetCategory(CategoryType type, int id)
		{
			return Categories.First(c => c.Type == type && c.Id == id);
		}

		public bool Equals(Category other)
		{
			return
				Type == other.Type
				&& Id == other.Id
			;
		}
	}
}
