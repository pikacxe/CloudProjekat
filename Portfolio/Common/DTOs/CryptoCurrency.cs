﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs
{
    public class CryptoCurrency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
}
