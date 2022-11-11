using System.ComponentModel.DataAnnotations;

namespace Thingsboard.Enums;

public enum UserTypeEnum
{
    [Display(Name = "tenant")]
    Tenant,
    [Display(Name = "customer")]
    Customer,
}
