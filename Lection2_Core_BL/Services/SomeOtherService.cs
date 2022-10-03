using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Services;

public class SomeOtherService
{
    private readonly CalculationService _service;

    public SomeOtherService(CalculationService service)
    {
        _service = service;
    }

    public int GetValue() => _service.Value;
}
