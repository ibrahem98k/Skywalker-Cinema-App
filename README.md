# Skywalker-Cinema-App
Skywalker Cinema is a fully-featured console-based cinema ticket booking system built with C#. 
It allows users to seamlessly browse movies, choose preferred showtimes, select ticket types, and pick their seats with a simple and intuitive console interface. 

Key features include:
- Multiple movies with multiple shows in different rooms and times.
- Interactive seat selection with clear indicators for selected, booked, regular, and deluxe seats.
- Support for various ticket types: Regular, Deluxe, AllDayRegular, and AllDayDeluxe, each with configurable pricing.
- Ability to apply discounts, either as a percentage or a fixed amount, at the time of booking.
- Unique ticket IDs generated for each booked seat.
- Booking cancellation by ticket ID, automatically updating the seat availability.
- Persistent data storage using JSON files (`movies.json` and `bookings.json`), allowing data to be saved and loaded between sessions.
- Default movies and shows initialized automatically if no data files exist, ensuring the system is ready to use out-of-the-box.
- Menu-driven, user-friendly console interface with input validation and helpful error messages.
- UTF-8 encoding for proper display of all characters in the console.

This project is perfect for learning how to manage complex data structures, work with collections like `List` and `Dictionary`, implement serialization with JSON, and build a fully interactive console application in C#.

It is ideal for students, hobbyists, and anyone interested in developing a real-world ticket booking system.
