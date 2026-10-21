using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.UnitTests.Domain
{
    public class ResultTests
    {
        private static readonly Error SomeError = Error.Conflict("Test.Conflict", "Algo chocó.");

        [Fact]
        public void Success_no_tiene_error()
        {
            var result = Result.Success();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(Error.None, result.Error);
        }

        [Fact]
        public void Failure_lleva_el_error_con_su_codigo_mensaje_y_tipo()
        {
            var result = Result.Failure(SomeError);

            Assert.True(result.IsFailure);
            Assert.Equal("Test.Conflict", result.Error.Code);
            Assert.Equal("Algo chocó.", result.Error.Message);
            Assert.Equal(ErrorType.Conflict, result.Error.Type);
        }

        [Fact]
        public void Success_con_valor_devuelve_el_valor()
        {
            var result = Result.Success(42);

            Assert.True(result.IsSuccess);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void Leer_el_valor_de_un_resultado_fallido_lanza_excepcion()
        {
            var result = Result.Failure<int>(SomeError);

            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void Un_valor_se_convierte_implicitamente_en_exito()
        {
            Result<string> result = "Tecnología";

            Assert.True(result.IsSuccess);
            Assert.Equal("Tecnología", result.Value);
        }

        [Fact]
        public void Un_error_se_convierte_implicitamente_en_fallo()
        {
            Result<string> result = SomeError;
            Result plain = SomeError;

            Assert.True(result.IsFailure);
            Assert.True(plain.IsFailure);
            Assert.Equal(SomeError, result.Error);
        }

        [Theory]
        [InlineData(ErrorType.Validation)]
        [InlineData(ErrorType.NotFound)]
        [InlineData(ErrorType.Conflict)]
        public void Las_fabricas_de_error_asignan_el_tipo(ErrorType type)
        {
            var error = type switch
            {
                ErrorType.Validation => Error.Validation("C", "M"),
                ErrorType.NotFound => Error.NotFound("C", "M"),
                _ => Error.Conflict("C", "M")
            };

            Assert.Equal(type, error.Type);
        }
    }
}
