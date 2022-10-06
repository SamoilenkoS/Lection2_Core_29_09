using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Options
{
    public class HashingOptions
    {
        public string? Salt { get; set; }
        public int IterationCount { get; set; }
        public int NumBytesRequested { get; set; }
    }
}
