using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Services;

public class CalculationService
{
    private int _value;

    public int Value
    {
        get
        {
            return _value++;
        }
    }
}
