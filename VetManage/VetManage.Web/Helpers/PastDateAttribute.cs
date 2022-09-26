using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Helpers
{
    public class PastDateAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly DateTime _now;
        public PastDateAttribute()
        {
            _now = DateTime.Now;
        }
        public override bool IsValid(object value)// Return a boolean value: true == IsValid, false != IsValid
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            DateTime date = Convert.ToDateTime(value);

            //Dates Greater than or equal to today are valid (true)
            return date >= _now;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-pastdate", ErrorMessage);
            context.Attributes.Add("data-val-pastdate-now", _now.ToString("MM/dd/yyyy"));
        }
    }
}