﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Smartplug.Core.Dtos
{
    public class Response<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; }

        [JsonIgnore]
        public bool IsSuccessful { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("Messages")]
        public List<string> Messages { get; set; }


        // Static Factory Method
        //public static Response<T> Success(T data, int statusCode, Pager pager = null)
        //{
        //    return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true, Pagination = pager };
        //}

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default(T), StatusCode = statusCode};
        }
        public static Response<T> Success(T data,int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode };
        }
        public static Response<T> Fail(List<string> errors, int statusCode)
        {
            return new Response<T>
            {
                Messages = errors,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

        //public static Response<T> Fail(ValidationResult result, int statusCode)
        //{
        //    return new Response<T>
        //    {
        //        Messages = result.Errors.Select(x => x.ErrorMessage).ToList(),
        //        StatusCode = statusCode,
        //        IsSuccessful = false
        //    };
        //}

        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T> { Messages = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
        }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public Pager? Pagination { get; set; }

    }
}
