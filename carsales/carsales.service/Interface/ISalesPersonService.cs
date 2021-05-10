using carsales.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace carsales.service.Interface
{
    public interface ISalesPersonService
    {
        SalesPersonModel GetAllSalesPerson();
        SalesPersonModel GetAll();
        SalesCustomerModel GetCustomerData();
        string AssignSalesPerson(PayloadModel payload);
    }
}
