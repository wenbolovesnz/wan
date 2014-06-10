using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wan.Services
{
    public class FileValidationService
    {
        public bool ValidateUpload(string fileName)
        {
            var isValid = true;

            var allowedValue = new string[] { "jpeg", "jpg", "gif", "png", "PNG" };

            var array = fileName.Split('.').ToList();

            if (!allowedValue.Contains(array.Last()))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}