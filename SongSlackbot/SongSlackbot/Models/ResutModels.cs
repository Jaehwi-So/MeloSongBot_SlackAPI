using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SongSlackbot.Models
{
    public class ResultModels<T>
    {
        public ResultModels(bool success, T data)
        {
            this.Success = success;
            this.ResultList = data;
        }
        public bool Success { get; set; }
        public T ResultList { get; set; }

    }
}