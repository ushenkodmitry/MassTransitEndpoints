namespace Options.MassTransit
{
    public sealed class BusOptions
    {
        public string Serializer { get; set; }

        public string QueueNameFormat { get; set; } = "bus-{MachineName}-{AssemblyName}-{NewId}";
    }
}
