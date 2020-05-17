using Newtonsoft.Json;

namespace Trading.Analytics.Shared.Models
{
    public class OpenApiResponse<T>
    {
        public string TrackingId { get; }
        public string Status { get; }
        public T Payload { get; }

        [JsonConstructor]
        public OpenApiResponse(string trackingId, string status, T payload)
        {
            TrackingId = trackingId;
            Status = status;
            Payload = payload;
        }
    }
}