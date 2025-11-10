using HotelBooking.Core.Entities;
using HotelBooking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.WebApi.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly IRepository<Customer> repository;

        public CustomersController(IRepository<Customer> repos)
        {
            repository = repos;
        }

        // GET: customers
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await repository.GetAllAsync();
        }

    }

