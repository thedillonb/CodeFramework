using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeFramework.Controllers
{
    public class ListModel<T>
    {
        public List<T> Data { get; set; }
        public List<IGrouping<string, T>> FilteredData { get; set; }
        public Action More { get; set; }

        public ListModel()
        {
        }

        public ListModel(List<T> data, Action more)
        {
            Data = data;
            More = more;
        }
    }
}

