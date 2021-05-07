using carsales.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace carsales.service.Interface
{
    public interface ISalesPersonService
    {
        SalesPersonModel GetAll();
        void AssignSalesPerson(CarType carType, Language lang);
    }
}
