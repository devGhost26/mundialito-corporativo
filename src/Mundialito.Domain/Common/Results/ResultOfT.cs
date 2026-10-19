using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Common.Results
{
    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(T? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public T value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("No se puede leer el valor de un resultado.");

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure<T>(error);


    }
}
