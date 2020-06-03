namespace Trading.Analytics.Shared.Models
{
    public class OpenApiError : OpenApiResponse<OpenApiError.Envelope>
    {
        public class Envelope
        {
            public string Message { get; }
            public string Code { get; }
        }

        public OpenApiError(string trackingId, string status, Envelope payload) : base(trackingId, status, payload)
        {
        }
    }
}