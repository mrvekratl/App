using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    namespace App.Services
    {
        public class Result
        {
            // Başarı durumunu kontrol etmek için public property
            public bool IsSuccess { get; private set; }

            // Hata mesajını taşıyan public property
            public string Error { get; private set; }

            // Başarı durumu için metot
            public static Result Success() => new Result { IsSuccess = true };

            // Hata durumu için metot
            public static Result Fail(string error) => new Result { IsSuccess = false, Error = error };
        }

        // Generic Result<T> sınıfı
        public class Result<T> : Result
        {
            // Data taşımak için public property
            public T Value { get; private set; }

            public static bool GetIsSuccess()
            {
                return IsSuccess;
            }

            // Başarı durumu ve veri ile metot
            public static Result<T> Success(T value, bool ısSuccess) => new Result<T> { IsSuccess = true, Value = value };

            // Hata durumu ve mesaj ile metot
            public static Result<T> Fail(string error) => new Result<T> { IsSuccess = false, Error = error };
        }
    }

}
