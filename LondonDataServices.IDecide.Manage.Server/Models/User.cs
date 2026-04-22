using System;

namespace LondonDataServices.IDecide.Manage.Server.Models;

public class User
{
    public int Id { get; set; }
    public string NhsIdUserUid { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Sub { get; set; } = default!;
    public string RawUserInfo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsAuthorised { get; set; } = false;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTimeOffset UpdatedDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}