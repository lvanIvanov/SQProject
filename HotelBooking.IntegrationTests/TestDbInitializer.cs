using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Infrastructure;
using HotelBooking.Core.Entities;

namespace HotelBooking.IntegrationTests;

public class TestDbInitializer : IDbInitializer
{
    public void Initialize(HotelBookingContext context)
    {
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        
        if (context.Booking.Any())
        {
            return; 
        }

        List<Customer> customers =
        [
            new Customer { Name="John Doe", Email="john@example.com" },
            new Customer { Name="Jane Doe", Email="jane@example.com" }
        ];

        List<Room> rooms =
        [
            new Room { Description="A" }
        ];

        DateTime date = DateTime.Today.AddDays(4);
        List<Booking> bookings =
        [
            new() { StartDate=DateTime.Parse("2025-11-15 12:00:00"), EndDate=DateTime.Parse("2025-11-17 12:00:00"), IsActive=true, CustomerId=2, RoomId=1 }
        ];

        context.Customer.AddRange(customers);
        context.Room.AddRange(rooms);
        context.SaveChanges();
        context.Booking.AddRange(bookings);
        context.SaveChanges();
    }
}