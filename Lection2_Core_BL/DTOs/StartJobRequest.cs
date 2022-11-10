using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.DTOs
{
    public class StartJobRequest
    {
        public string JobTitle { get; set; }
        public int IntervalInSeconds { get; set; }
        public int RepeatCount { get; set; }
    }
}
