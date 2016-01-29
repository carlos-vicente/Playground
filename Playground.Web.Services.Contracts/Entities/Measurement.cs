namespace Playground.Web.Services.Contracts.Entities
{
    public class Measurement
    {
        public int Id { get; set; }

        public MeasurementTypes Type { get; set; }

        public decimal Value { get; set; }
    }
}
