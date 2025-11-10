Feature: Create Booking

    As a user
    I want to create a booking
    So that I can reserve a room
    
    Background: Setup existing bookings
        Given the occupied period is from "2025-11-15 12:00:00" to "2025-11-20 12:00:00"
        
        
    Scenario: Successfully create bookings with the available rooms
        Given a new booking is requested from "<startDate>", to "<endDate>", in room number "<roomID>" by customer number "<customerID>"
        When I create the booking
        Then the booking should be created successfully
        
       
    Examples:
      | startDate          | endDate            | roomID | customerID |  
      | 2025-11-10 12:00:00 | 2025-11-14 12:00:00 | 1       | 1           | 
      | 2025-11-21 12:00:00 | 2026-11-25 12:00:00 | 1       | 1           | 

         
    Scenario: Booking fails due to no available rooms
        Given that I already have booking from "<startDate>" to "<endDate>". in room number "<roomID>" by customer number "<customerID>"
        When a new booking is requested
        Then the booking should not be created
        
        
    Examples:
      | startDate          | endDate            | roomID | customerID |
      | 2025-11-13 12:00:00 | 2025-11-17 12:00:00 | 1       | 1           |
      | 2025-11-19 12:00:00 | 2025-11-22 12:00:00 | 1       | 1           |
      
    Scenario: Booking fails due to overlapping dates
        Given that I already have booking from "<startDate>" to "<endDate>". in room number "<roomID>" by customer number "<customerID>"
        When a new booking is requested
        Then the booking creation should fail due to overlapping dates

    Examples:
      | startDate          | endDate            | roomID | customerID |
      | 2025-11-17 12:00:00 | 2025-11-17 12:00:00 | 1       | 1           |
      | 2025-11-18 12:00:00 | 2025-11-18 12:00:00 | 1       | 1           |
      | 2025-11-17 12:00:00 | 2025-11-18 12:00:00 | 1       | 1           |