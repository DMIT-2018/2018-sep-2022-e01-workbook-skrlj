using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinookSystem.ViewModels
{
    // Record is an unchangable value. Read Only!
    public record NamedColor(string RgbCode, string HexCode, string Name, int ColorType, bool Available)
    {
       
    }
}
