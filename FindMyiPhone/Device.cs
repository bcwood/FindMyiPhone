namespace FindMyiPhone
{
    public class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DeviceDisplayName { get; set; }
        public string ModelDisplayName { get; set; }
        public string RawDeviceModel { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceClass { get; set; }
        public string DeviceStatus { get; set; }

        public decimal BatteryLevel { get; set; }
        public string BatteryStatus { get; set; }

        public bool CanWipeAfterLock { get; set; }
        public bool WipeInProgress { get; set; }
        public bool LostModeEnabled { get; set; }
        public bool ActivationLocked { get; set; }
        public bool LowPowerMode { get; set; }
        public bool IsLocating { get; set; }
        public bool LocationEnabled { get; set; }
        public bool LocFoundEnabled { get; set; }
        public bool FmlyShare { get; set; }
        public bool LostModeCapable { get; set; }
        public bool LocationCapable { get; set; }

        public string LostDevice { get; set; }
        public string LostTimestamp { get; set; }
        public string LockedTimestamp { get; set; }
        public string WipedTimestamp { get; set; }
        
        public Location Location { get; set; }
    }
}
