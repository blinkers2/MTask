using System.Text.Json.Serialization;

namespace MTask.Data.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        public decimal PercentageInWholePopulation { get; set; }
    }
}
