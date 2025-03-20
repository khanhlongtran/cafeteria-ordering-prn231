namespace CafeteriaOrdering.API.DTO
{
    public class UpdateGeoLocationRequest
    {
        public string GeoLocation { get; set; } = null!;
        public string? Image { get; set; }
    }

}
