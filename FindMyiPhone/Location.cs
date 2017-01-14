namespace FindMyiPhone
{
    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal HorizontalAccuracy { get; set; }
        public string PositionType { get; set; }
        public string LocationType { get; set; }
        public long Timestamp { get; set; }

        public bool IsOld { get; set; }
        public bool IsInaccurate { get; set; }
        public bool LocationFinished { get; set; }
    }
}
