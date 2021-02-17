namespace FindMyChair.Models.Mapping
{
    public class Locations
    {
        public int LocationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Locations()
        {

        }

        public Locations(int locid, string title, string desc, double latitude, double longitude)
        {
            LocationId = locid;
            Title = title;
            Description = desc;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}