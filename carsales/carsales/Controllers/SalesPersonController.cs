using carsales.data.Models;
using carsales.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace carsales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesPersonController : ControllerBase
    {
        private readonly ISalesPersonService _salesPersonService;
        public SalesPersonController(ISalesPersonService salesPersonService)
        {
            _salesPersonService = salesPersonService;
        }

        // GET: api/<SalesPersonController>
        [HttpGet]
        public SalesPersonModel Get()
        {
            var salesPersons = _salesPersonService.GetAll();

            return salesPersons;
        }

        [HttpGet("customer-data")]
        public SalesCustomerModel GetCustomerData()
        {
            var salesPersons = _salesPersonService.GetCustomerData();

            return salesPersons;
        }

        // GET api/<SalesPersonController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SalesPersonController>
        [HttpPost]
        public string Post([FromBody]PayloadModel payload)
        {
            return _salesPersonService.AssignSalesPerson(payload);
        }

        // PUT api/<SalesPersonController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SalesPersonController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
