namespace suffusive.uveitis.Dependencies.Interfaces
{
    public interface IConfiguration
    {
        string SupportEmail { get; set; }
        string Signature { get; set; }
        float TaxRate { get; set; }
    }
}