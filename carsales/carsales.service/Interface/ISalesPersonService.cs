using carsales.data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace carsales.service.Interface
{
    public interface ISalesPersonService
    {
        SalesPersonModel GetAllSalesPerson();
        SalesPersonModel GetAll();
        SalesCustomerModel GetCustomerData();
        Task<string> AssignSalesPerson(PayloadModel payload);
    }
}
