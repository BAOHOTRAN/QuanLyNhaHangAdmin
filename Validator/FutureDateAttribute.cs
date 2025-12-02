using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangAdmin.Validator
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dt)
            {
                if (dt.Date < DateTime.Now.Date)
                    return new ValidationResult(ErrorMessage ?? "Ngày không được là ngày trong quá khứ");
            }
            return ValidationResult.Success;
        }
    }
}

