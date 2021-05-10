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

namespace carsales.service.Service
{
    public class SalesPersonService : ISalesPersonService
    {

        private static string destinationPath = @"..\..\..\carsales_BE\carsales\carsales.data\Data\customer-salesPerson.json";
        private static string destinationJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\customer-salesPerson.json");

        private static string sourcePath = @"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json";
        private static string sourceJsonString = File.ReadAllText(@"..\..\..\carsales_BE\carsales\carsales.data\Data\salesperson.json");

        private List<SalesPerson> salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);
        private List<SalesCustomer> customerData = JsonSerializer.Deserialize<List<SalesCustomer>>(destinationJsonString);
        private SalesPerson person = new SalesPerson();
        private SalesCustomer customer = new SalesCustomer();

        public string AssignSalesPerson(PayloadModel payload)
        {
            return AssignPerson(payload);
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
            var salesPersons = JsonSerializer.Deserialize<List<SalesPerson>>(sourceJsonString);

            SalesPersonModel persons = new SalesPersonModel();
            persons.SalesPerson = salesPersons;

            return persons;
        }

        public SalesCustomerModel GetCustomerData()
        {
            var customers = JsonSerializer.Deserialize<List<SalesCustomer>>(destinationJsonString);

            SalesCustomerModel customerModel = new SalesCustomerModel();
            customerModel.Customers = customers;

            return customerModel;
        }

        private string AssignPerson(PayloadModel payload)
        {
            string result = string.Empty;
            switch (payload.CarType)
            {
                case CarType.SportsCar:
                    result = UpdateStatus(GroupConstants.B, payload);
                    break;
                case CarType.FamilyCar:
                    result = UpdateStatus(GroupConstants.C, payload);
                    break;
                case CarType.TradieVehicle:
                    result = UpdateStatus(GroupConstants.D, payload);
                    break;
                case CarType.Unspecific:
                    result = UpdateStatus(GroupConstants.NA, payload);
                    break;
                default:
                    break;
            }
            return result;
        }
        
        private string UpdateStatus(string group, PayloadModel payload)
        {
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
                    person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group));

                if (person == null)
                    return GroupConstants.SalesPersonBusy;

            } while (personFound);
            
                        
            salesPersons.Remove(person);
            salesPersons.Add(new SalesPerson()
            {
                Name = person.Name,
                Groups = person.Groups,
                isAvailable = false
            });

            string json = JsonSerializer.Serialize(salesPersons);
            File.WriteAllText(sourcePath, json);
            
            var dt = new SalesCustomer
            {
                CustomerName = payload.Customer,
                SalesName = person.Name,
                Language = payload.speaksGreek ? "Greek" : "Non-Greek speaking",
                Car = MapCar(payload.CarType)
            };

            Save(dt);

            return string.Format("{0} was assigned. Thank you.", person.Name);
        }

        private SalesPerson LookforSalesPerson(string group, bool isGreekSpeaking)
        {
            if (isGreekSpeaking)
                person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group) && x.Groups.Contains(GroupConstants.A));
            else
                person = salesPersons.FirstOrDefault(x => x.isAvailable && x.Groups.Contains(group));
                        
            return person;
        }
        private SalesPerson GetRandomSalesPerson()
        {
            // get the first available sales person when there is no available for the given group.
            // No sense to create extra logic for getting random available sales person.
            return person = salesPersons.FirstOrDefault(x => x.isAvailable);
        }

        private void Save(SalesCustomer salesCustomer)
        {
            if (customerData != null)
                customerData.Remove(customer); 
            
            customerData.Add(new SalesCustomer()
            {
                CustomerName = salesCustomer.CustomerName,
                Car = salesCustomer.Car,
                Language = salesCustomer.Language,
                SalesName = salesCustomer.SalesName
            });

            string json = JsonSerializer.Serialize(customerData);
            File.WriteAllText(destinationPath, json);
        }        

        private string MapCar(CarType carType)
        {
            CarType car = carType;
            return car.ToString();
        }
    }
}
