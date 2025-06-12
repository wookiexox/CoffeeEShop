namespace CoffeeEShop.Models;

public class EmailTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public EmailTemplateType Type { get; set; }
}

public enum EmailTemplateType
{
    EmailConfirmation = 0,
    PasswordReset = 1,
    OrderConfirmation = 2,
    Invoice = 3
}
