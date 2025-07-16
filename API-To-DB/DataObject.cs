namespace API_To_DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // The DataObject used to store the attributes from the API response
    public class DataObject
    {
        required public string Name { get; set; }

        required public string Address { get; set; }

        required public string Zip { get; set; }

        required public string Country { get; set; }

        required public string EmployeeCount { get; set; }

        required public string Industry { get; set; }

        required public string MarketCap { get; set; }

        required public string Domain { get; set; }

        required public string Logo { get; set; }

        required public string CeoName { get; set; }
    }
}
