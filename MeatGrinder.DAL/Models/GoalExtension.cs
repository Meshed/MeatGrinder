using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeatGrinder.DAL.Models
{
    public partial class Goal
    {
        public int ChildTaskCount { get; set; }
    }
}