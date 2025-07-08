using VSADemo.Common.Domain.Base.Interfaces;

namespace VSADemo.Common.Domain.Managers;

public record Email : IValueObject
{
  public const int EmailMaxLength = 255;

  private string _emailAddress = null!;

  public string EmailAddress
  {
    get => _emailAddress;
    private set
    {
      ThrowIfNullOrWhiteSpace(value, nameof(EmailAddress));
      ThrowIfGreaterThan(value.Length, EmailMaxLength, nameof(EmailAddress));
      if (!IsValidEmail(value))
      {
        throw new ArgumentException("Invalid email format.", nameof(EmailAddress));
      }
      _emailAddress = value;
    }
  }

  public Email(string emailAddress)
  {
    EmailAddress = emailAddress;
  }

  private static bool IsValidEmail(string email)
  {
    try
    {
      var addr = new System.Net.Mail.MailAddress(email);
      return addr.Address == email;
    }
    catch
    {
      return false;
    }
  }
}