﻿using System;

namespace Equinor.ProCoSys.PCS5.Command.Validators.RowVersionValidators
{
    public class RowVersionValidator : IRowVersionValidator
    {
        public bool IsValid(string rowVersion)
            => !string.IsNullOrWhiteSpace(rowVersion) && TryConvertBase64StringToByteArray(rowVersion);

        private static bool TryConvertBase64StringToByteArray(string input)
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
