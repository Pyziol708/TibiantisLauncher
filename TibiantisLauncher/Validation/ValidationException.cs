﻿using System;

namespace TibiantisLauncher.Validation
{
    internal class ValidationException : Exception
    {
        public ValidationException() : base() { }
        public ValidationException(string? message) : base(message) { }
        public ValidationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
