using System;
using System.ComponentModel.DataAnnotations;

public class QuanTriNhaHangAdmin : ValidationAttribute
{
    public class ValidTimeAttribute : ValidationAttribute
    {
        private readonly TimeSpan _min;
        private readonly TimeSpan _max;

        public ValidTimeAttribute(string minTime, string maxTime)
        {
            _min = TimeSpan.Parse(minTime);
            _max = TimeSpan.Parse(maxTime);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is TimeSpan ts)
            {
                if (ts < _min || ts > _max)
                    return new ValidationResult(ErrorMessage ?? $"Giờ phải từ {_min:hh\\:mm} đến {_max:hh\\:mm}");
            }
            return ValidationResult.Success;
        }
    }
}
