using carsales.data;
using carsales.data.Entity;
using carsales.data.Models;
using carsales.service.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace carsales.service.Service
{
    public class SalesPersonService : ISalesPersonService
    {

        SalesPerson person = new SalesPerson();

        public async Task<string> AssignSalesPerson(PayloadModel payload)
        {
            return await AssignPerson(payload);
        }

        public SalesPersonModel GetAll()
        {

            throw new NotImplementedException();
        }

        public SalesPersonModel GetAllSalesPerson()
        {
            return GetSalesPerson();
        }

        private SalesPersonModel GetSalesPerson()
        {
            string sourceJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json");
            var salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);

            SalesPersonModel persons = new SalesPersonModel();
            persons.SalesPerson = salesPersons;

            return persons;
        }

        public SalesCustomerModel GetCustomerData()
        {
            string destinationJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\customer-salesPerson.json");
            var customers = JsonSerializer.Deserialize<List<SalesCustomer>>(destinationJsonString);

            SalesCustomerModel customerModel = new SalesCustomerModel();
            customerModel.Customers = customers;

            return customerModel;
        }

        private async Task<string> AssignPerson(PayloadModel payload)
        {
            string result = string.Empty;
            switch (payload.CarType)
            {
                case CarType.SportsCar:
                    result = await UpdateStatus(GroupConstants.B, payload);
                    break;
                case CarType.FamilyCar:
                    result = await UpdateStatus(GroupConstants.C, payload);
                    break;
                case CarType.TradieVehicle:
                    result = await UpdateStatus(GroupConstants.D, payload);
                    break;
                case CarType.Unspecific:
                    result = await UpdateStatus(GroupConstants.NA, payload);
                    break;
                default:
                    break;
            }
            return result;
        }

        private async Task<string> UpdateStatus(string group, PayloadModel payload)
        {
            string sourceJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json");
            string sourcePath = @"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json";
            var salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);
            
            bool personFound = false;
            do
            {
                if (payload.speaksGreek)
                {
                    person = LookforSalesPerson(group, true);
                    if (person != null)
                    {
                        personFound = true;
                        break;
                    }
                    else
                    {
                        person = LookforSalesPerson(group, false);
                        if (person != null)
                        {
                            personFound = true;
                            break;
                        }
                    }
                    if (person == null)
                        person = GetRandomSalesPerson();
                }
                else
                {
                    person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group));
                    if (person != null)
                    {
                        personFound = true;
                        break;
                    }
                    else
                    {
                        person = GetRandomSalesPerson();
                        if (person != null)
                        {
                            personFound = true;
                            break;
                        }
                    }
                }
                    

                if (person == null)
                    return GroupConstants.SalesPersonBusy;

            } while (personFound);


            var check = salesPersons.Remove(salesPersons.FirstOrDefault(x => x.Name == person.Name));
            salesPersons.Add(new SalesPerson()
            {
                Name = person.Name,
                Groups = person.Groups,
                isAvailable = false
            });
            
            string newJson = JsonSerializer.Serialize(salesPersons);
            await File.WriteAllTextAsync(sourcePath, newJson);
            var dt = new SalesCustomer
            {
                CustomerName = payload.Customer,
                SalesName = person.Name,
                Language = payload.speaksGreek ? "Greek" : "Non-Greek speaking",
                Car = MapCar(payload.CarType)
            };

            //save customer data
            Save(dt);

            return string.Format("{0} was assigned. Thank you.", person.Name);
        }

        private SalesPerson LookforSalesPerson(string group, bool isGreekSpeaking)
        {
            string sourceJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json");
            var salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);
            SalesPerson person = new SalesPerson();

            if (isGreekSpeaking)
                person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group) && x.Groups.Contains(GroupConstants.A));
            else
                person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group));

            return person;
        }
        private SalesPerson GetRandomSalesPerson()
        {
            string sourceJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json");
            var salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);
            SalesPerson person = new SalesPerson();
            // get the first available sales person when there is no available for the given group.
            // No sense to create extra logic for getting random available sales person.
            return person = salesPersons.FirstOrDefault(x => x.isAvailable);
        }

        private async void Save(SalesCustomer salesCustomer)
        {
            string destinationPath = @"..\..\..\carsales_BE\carsales\carsales.data\Data\customer-salesPerson.json";
            string destinationJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\customer-salesPerson.json");
            var customerData = JsonSerializer.Deserialize<List<SalesCustomer>>(destinationJsonString);
            SalesCustomer customer = new SalesCustomer();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (customerData != null)
                customerData.Remove(customer);
            
            customerData.Add(new SalesCustomer()
            {
                CustomerName = salesCustomer.CustomerName,
                Car = salesCustomer.Car,
                Language = salesCustomer.Language,
                SalesName = salesCustomer.SalesName
            });

            string newJson = JsonSerializer.Serialize(customerData, options);
            File.WriteAllText(destinationPath, newJson);
        }

        private string MapCar(CarType carType)
        {
            CarType car = carType;
            return car.ToString();
        }
    }
}
