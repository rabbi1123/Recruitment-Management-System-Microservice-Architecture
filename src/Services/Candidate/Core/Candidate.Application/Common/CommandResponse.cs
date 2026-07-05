using System.Text.Json.Serialization;

namespace Candidate.Application.Common
{
    public class CommandResponse
    {
        public bool IsSuccess { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id { get; set; }
    }
}
