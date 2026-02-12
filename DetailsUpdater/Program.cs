using Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailsUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new InventoryRepo().UpdatePricing("magic the gathering");
        }
    }
}
