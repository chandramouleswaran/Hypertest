﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypertest.WebTest.Utils
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FindRow
    {
        public FindRow()
        {
        }

        public FindRow(string theKey, int theKeyColumn)
        {
            Key = theKey;
            KeyColumn = theKeyColumn;
        }

        public string Key { get; set; }
        public int KeyColumn { get; set; }
    }
}