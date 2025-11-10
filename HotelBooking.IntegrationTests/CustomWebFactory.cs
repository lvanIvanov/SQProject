using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;

namespace HotelBooking.IntegrationTests;


public class CustomWebAppFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<HotelBookingContext>));
            
                services.Remove(descriptor);

            services.AddDbContext<HotelBookingContext>(options =>
            {
                options.UseInMemoryDatabase("HotelBookingTestDb");
            });
        });
        
        builder.UseEnvironment("Development");
    }
}