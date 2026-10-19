using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Common.Results
{
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static Error Validation(string Code, string Message) =>
            new(Code, Message, ErrorType.Validation);
        public static Error NotFound(string Code, string Message) =>
            new(Code, Message, ErrorType.NotFound);
        public static Error Confilct(string Code, string Message) =>
            new(Code, Message, ErrorType.Conflict);
        public static Error Failure(string Code, string Message) =>
            new(Code, Message, ErrorType.Failure);
    }
}
