﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Numeric values are for Borgun Api, string value is used in other providers.
    /// </summary>
    public enum Currency
    {
        DKK = 208,
        ISK = 352,
        GBP = 826,
        USD = 840,
        EUR = 978,
    }
}