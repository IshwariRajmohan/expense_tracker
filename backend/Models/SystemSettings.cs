namespace backend.Models;

public class SystemSettings
{
    public int Id { get; set; } = 1;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CorporateCurrency { get; set; } = string.Empty;
    public string SystemMode { get; set; } = string.Empty;
}
