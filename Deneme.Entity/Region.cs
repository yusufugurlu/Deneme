﻿using Deneme.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deneme.Entity
{
    [Table(PrimaryColumn = "RegionID", TableName = "Region")]
    public class Region
    {
        public int RegionID { get; set; }
        public string RegionDescription { get; set; }
    }
}
