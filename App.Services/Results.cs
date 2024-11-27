using App.Models.DTO;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    // Result.cs
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Error { get; protected set; }

        // Başarı durumunu döndüren metot
        public static Result Success() => new Result { IsSuccess = true };

        // Hata durumunu döndüren metot
        public static Result Fail(string error) => new Result { IsSuccess = false, Error = error };
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }

        // Başarı durumu ve veri için metot
        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };

        // Hata durumu için metot
        public static Result<T> Fail(string error) => new Result<T> { IsSuccess = false, Error = error };
    }



}





