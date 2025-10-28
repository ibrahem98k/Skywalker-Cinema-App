using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Skywalker.cinema
{
    enum TicketType { Regular, Deluxe, AllDayRegular, AllDayDeluxe }

    class Seat
    {
        public string Row { get; set; }
        public int Number { get; set; }
    }

    class Show
    {
        public int Room { get; set; }
        public TimeSpan Time { get; set; }
        public int Duration { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
        public Dictionary<DayOfWeek, Dictionary<TicketType, List<string>>> DailyBookedSeats { get; set; }
            = new Dictionary<DayOfWeek, Dictionary<TicketType, List<string>>>();

        public void InitializeDailySeats()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                DailyBookedSeats[day] = new Dictionary<TicketType, List<string>>();
                foreach (TicketType type in Enum.GetValues(typeof(TicketType)))
                    DailyBookedSeats[day][type] = new List<string>();
            }
        }
    }

    class Movie
    {
        public string Title { get; set; }
        public List<Show> Shows { get; set; } = new List<Show>();
    }

    class Booking
    {
        public string MovieTitle { get; set; }
        public int Room { get; set; }
        public DateTime ShowTime { get; set; }
        public TicketType TicketType { get; set; }
        public DayOfWeek Day { get; set; }
        public Dictionary<string, string> SeatsWithIds { get; set; } = new Dictionary<string, string>();
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; } = 0;
    }

    class Program
    {
        static string moviesFile = "movies.json";
        static string bookingsFile = "bookings.json";

        static Dictionary<TicketType, decimal> ticketPrices = new Dictionary<TicketType, decimal>
        {
            { TicketType.Regular, 5000m },
            { TicketType.Deluxe, 8000m },
            { TicketType.AllDayRegular, 10000m },
            { TicketType.AllDayDeluxe, 15000m }
        };

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Movie> movies = LoadMovies();
            List<Booking> bookings = LoadBookings();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to SKYWALKER CINEMA !");
                Console.WriteLine("1. Make a new booking");
                Console.WriteLine("2. Cancel a booking");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string option = Console.ReadLine();

                if (option == "0") break;
                else if (option == "1")
                {
                    Movie movie = ChooseMovie(movies);
                    if (movie == null) continue;

                    TicketType? ticketType = ChooseTicketType();
                    if (ticketType == null) continue;

                    DayOfWeek selectedDay = ChooseDay();
                    if (selectedDay == (DayOfWeek)(-1)) continue;

                    Show show = ChooseShow(movie);
                    if (show == null) continue;

                    int? tickets = ChooseNumberOfSeats(show, ticketType.Value, selectedDay);
                    if (tickets == null) continue;

                    Console.WriteLine("\nChoose your seats:");
                    List<string> selectedSeats = new List<string>();

                    for (int i = 0; i < tickets.Value; i++)
                    {
                        DisplayRoom(show, ticketType.Value, selectedSeats, selectedDay, movie.Title);
                        Console.Write($"\nChoose seat {i + 1} of {tickets.Value} (e.g., A1) (0 to go back): ");

                        string input = Console.ReadLine().ToUpper();
                        if (input == "0") break;

                        if (!ValidSeatCode(input) || IsSeatBooked(show, ticketType.Value, input, selectedDay))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid choice!");
                            Console.ResetColor();
                            i--;
                            continue;
                        }

                        selectedSeats.Add(input);
                        AddSeatBooking(show, ticketType.Value, input, selectedDay);
                        DisplayRoom(show, ticketType.Value, selectedSeats, selectedDay, movie.Title);
                    }

                    if (selectedSeats.Count == 0) continue;

                    Dictionary<string, string> seatsWithIds = new Dictionary<string, string>();
                    foreach (var seat in selectedSeats)
                        seatsWithIds[seat] = Guid.NewGuid().ToString();

                    decimal totalPrice = selectedSeats.Count * ticketPrices[ticketType.Value];
                    decimal discount = AskDiscount(totalPrice);
                    totalPrice -= discount;

                    bookings.Add(new Booking
                    {
                        MovieTitle = movie.Title,
                        Room = show.Room,
                        ShowTime = DateTime.Today.Add(show.Time),
                        TicketType = ticketType.Value,
                        Day = selectedDay,
                        SeatsWithIds = seatsWithIds,
                        TotalPrice = totalPrice,
                        Discount = discount
                    });

                    Console.WriteLine($"\nBooking completed! Total Price: {totalPrice} (Discount: {discount})");
                    foreach (var seat in seatsWithIds)
                        Console.WriteLine($"Seat {seat.Key} - Ticket ID: {seat.Value}");

                    Console.WriteLine("Press any key to return to menu...");
                    Console.ReadKey();
                }
                else if (option == "2")
                {
                    Console.Write("\nEnter the Ticket ID to cancel: ");
                    string ticketId = Console.ReadLine();
                    bool found = false;

                    for (int i = 0; i < bookings.Count; i++)
                    {
                        var booking = bookings[i];
                        foreach (var seat in new Dictionary<string, string>(booking.SeatsWithIds))
                        {
                            if (seat.Value == ticketId)
                            {
                                var show = FindShow(movies, booking.MovieTitle, booking.Room, booking.ShowTime.TimeOfDay);
                                if (show != null)
                                    RemoveSeatBooking(show, booking.TicketType, seat.Key, booking.Day);

                                booking.SeatsWithIds.Remove(seat.Key);
                                if (booking.SeatsWithIds.Count == 0)
                                    bookings.RemoveAt(i);

                                found = true;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Booking cancelled successfully!");
                                Console.ResetColor();
                                break;
                            }
                        }
                        if (found) break;
                    }

                    if (!found)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ticket ID not found!");
                        Console.ResetColor();
                    }

                    Console.WriteLine("Press any key to return to menu...");
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice!");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }

            SaveBookings(bookings);
            Console.WriteLine("All bookings saved. Goodbye!");
        }

        static Show FindShow(List<Movie> movies, string title, int room, TimeSpan time)
        {
            foreach (var movie in movies)
                if (movie.Title == title)
                    foreach (var show in movie.Shows)
                        if (show.Room == room && show.Time == time)
                            return show;
            return null;
        }

        static DayOfWeek ChooseDay()
        {
            while (true)
            {
                Console.WriteLine("\nChoose a day to book:");
                int idx = 1;
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                    Console.WriteLine($"{idx++}. {day}");
                Console.WriteLine("0. Go Back");

                if (int.TryParse(Console.ReadLine(), out int dayChoice))
                {
                    if (dayChoice == 0) return (DayOfWeek)(-1);
                    if (dayChoice >= 1 && dayChoice <= 7)
                        return (DayOfWeek)((dayChoice) % 7);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice! Please try again.");
                Console.ResetColor();
            }
        }

        static void DisplayRoom(Show show, TicketType ticketType, List<string> selectedSeats, DayOfWeek day, string movieTitle)
        {
            Console.Clear();
            Console.WriteLine($" {movieTitle}");
            Console.WriteLine($"Room: {show.Room}, Time: {show.Time}");
            Console.WriteLine($"Day: {day}, Ticket: {ticketType}\n");

            string[] rows = { "A", "B", "C", "D", "E" };

            Console.Write("   ");
            for (int n = 1; n <= 5; n++)
                Console.Write($" {n}  ");
            Console.WriteLine();

            foreach (var row in rows)
            {
                Console.Write(row + "  ");
                for (int n = 1; n <= 5; n++)
                {
                    string seatId = row + n;

                    if (selectedSeats.Contains(seatId))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[✓]");
                    }
                    else if (IsSeatBooked(show, ticketType, seatId, day))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[X]");
                    }
                    else
                    {
                        if (ticketType == TicketType.Regular || ticketType == TicketType.AllDayRegular)
                            Console.ForegroundColor = ConsoleColor.Gray;
                        else
                            Console.ForegroundColor = ConsoleColor.Yellow;

                        string display = (ticketType == TicketType.Regular || ticketType == TicketType.AllDayRegular) ? "R" : "D";
                        Console.Write($"[{display}]");
                    }

                    Console.ResetColor();
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n[✓] Selected, [X] Booked, [R] Regular, [D] Deluxe");
        }

        static bool ValidSeatCode(string code)
        {
            if (code.Length < 2) return false;
            string row = code.Substring(0, 1);
            if (!"ABCDE".Contains(row)) return false;
            if (!int.TryParse(code.Substring(1), out int num) || num < 1 || num > 5) return false;
            return true;
        }

        static decimal AskDiscount(decimal totalPrice)
        {
            Console.Write("\nDo you want a discount? (y/n): ");
            string wantDiscount = Console.ReadLine().Trim().ToLower();
            if (wantDiscount != "y") return 0;

            while (true)
            {
                Console.WriteLine("Discount type: 1. Percentage  2. Fixed amount");
                string input = Console.ReadLine().Trim().ToLower();
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1 && (parts[0] == "1" || parts[0] == "2"))
                {
                    if (parts[0] == "1")
                    {
                        Console.Write("Enter discount percentage (0-100): ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal perc) && perc >= 0 && perc <= 100)
                            return totalPrice * perc / 100;
                    }
                    else if (parts[0] == "2")
                    {
                        Console.Write("Enter fixed discount amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal fixedDisc) && fixedDisc >= 0)
                            return fixedDisc;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice! Write '1 Percentage' or '2 Fixed'.");
                Console.ResetColor();
            }
        }

        static List<Movie> LoadMovies()
        {
            if (!File.Exists(moviesFile))
            {
                List<Movie> defaultMovies = new List<Movie>
                {
                    new Movie{ Title="Avengers"},
                    new Movie{ Title="The Batman"},
                    new Movie{ Title="Spider-Man"},
                    new Movie{ Title="Inception"},
                    new Movie{ Title="Joker"}
                };

                string[] rows = { "A", "B", "C", "D", "E" };

                foreach (var movie in defaultMovies)
                    for (int room = 1; room <= 3; room++)
                        for (int showCount = 0; showCount < 4; showCount++)
                        {
                            DateTime showTime = DateTime.Today.AddHours(10 + (room - 1) + showCount * 3);
                            var show = new Show { Room = room, Time = showTime.TimeOfDay, Duration = 120 };
                            show.InitializeDailySeats();

                            foreach (var row in rows)
                                for (int n = 1; n <= 5; n++)
                                    show.Seats.Add(new Seat { Row = row, Number = n });

                            movie.Shows.Add(show);
                        }

                SaveMovies(defaultMovies);
                return defaultMovies;
            }
            else
            {
                string json = File.ReadAllText(moviesFile);
                var movies = JsonSerializer.Deserialize<List<Movie>>(json);

                foreach (var movie in movies)
                    foreach (var show in movie.Shows)
                        if (show.DailyBookedSeats == null || show.DailyBookedSeats.Count == 0)
                            show.InitializeDailySeats();

                return movies;
            }
        }

        static void SaveMovies(List<Movie> movies)
        {
            string json = JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(moviesFile, json);
        }

        static List<Booking> LoadBookings()
        {
            if (!File.Exists(bookingsFile)) return new List<Booking>();
            string json = File.ReadAllText(bookingsFile);
            return JsonSerializer.Deserialize<List<Booking>>(json);
        }

        static void SaveBookings(List<Booking> bookings)
        {
            string json = JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(bookingsFile, json);
        }

        static Movie ChooseMovie(List<Movie> movies)
        {
            while (true)
            {
                Console.WriteLine("\nPlease choose a movie from the list below");

                for (int i = 0; i < movies.Count; i++)
                    Console.WriteLine($"{i + 1}. {movies[i].Title}");
                Console.WriteLine("0. Go Back");

                if (int.TryParse(Console.ReadLine(), out int choice))
                    if (choice == 0) return null;
                    else if (choice >= 1 && choice <= movies.Count)
                        return movies[choice - 1];

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
            }
        }

        static TicketType? ChooseTicketType()
        {
            while (true)
            {
                Console.WriteLine("\nChoose your ticket type:");
                int idx = 1;

                foreach (var t in Enum.GetValues(typeof(TicketType)))
                    Console.WriteLine($"{idx++}. {t} - Price: {ticketPrices[(TicketType)t]}");

                Console.WriteLine("0. Go Back");

                if (int.TryParse(Console.ReadLine(), out int choice))
                    if (choice == 0) return null;
                    else if (choice >= 1 && choice <= 4)
                        return (TicketType)(choice - 1);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
            }
        }

        static Show ChooseShow(Movie movie)
        {
            Console.WriteLine("\nPlease choose a show time from the list below:");

            while (true)
            {
                for (int i = 0; i < movie.Shows.Count; i++)
                {
                    DateTime endTime = DateTime.Today.Add(movie.Shows[i].Time).AddMinutes(movie.Shows[i].Duration);
                    Console.WriteLine($"{i + 1}. Room {movie.Shows[i].Room} - Start: {movie.Shows[i].Time:hh\\:mm} - End: {endTime:HH:mm}");
                }

                Console.WriteLine("0. Go Back");

                if (int.TryParse(Console.ReadLine(), out int choice))
                    if (choice == 0) return null;
                    else if (choice >= 1 && choice <= movie.Shows.Count)
                        return movie.Shows[choice - 1];

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
            }
        }

        static int? ChooseNumberOfSeats(Show show, TicketType ticketType, DayOfWeek day)
        {
            while (true)
            {
                int availableSeats = 0;
                string[] rows = { "A", "B", "C", "D", "E" };

                foreach (var row in rows)
                    for (int n = 1; n <= 5; n++)
                        if (!IsSeatBooked(show, ticketType, row + n, day))
                            availableSeats++;

                Console.WriteLine($"\nAvailable seats: {availableSeats}");
                Console.Write("How many seats do you want? (0 to go back): ");

                if (int.TryParse(Console.ReadLine(), out int tickets))
                    if (tickets == 0) return null;
                    else if (tickets > 0 && tickets <= availableSeats)
                        return tickets;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
            }
        }

        static bool IsSeatBooked(Show show, TicketType type, string seat, DayOfWeek day)
        {
            if (type == TicketType.Regular || type == TicketType.AllDayRegular)
                return show.DailyBookedSeats[day][TicketType.Regular].Contains(seat) ||
                       show.DailyBookedSeats[day][TicketType.AllDayRegular].Contains(seat);

            if (type == TicketType.Deluxe || type == TicketType.AllDayDeluxe)
                return show.DailyBookedSeats[day][TicketType.Deluxe].Contains(seat) ||
                       show.DailyBookedSeats[day][TicketType.AllDayDeluxe].Contains(seat);

            return false;
        }

        static void AddSeatBooking(Show show, TicketType type, string seat, DayOfWeek day)
        {
            if (type == TicketType.Regular || type == TicketType.AllDayRegular)
            {
                show.DailyBookedSeats[day][TicketType.Regular].Add(seat);
                show.DailyBookedSeats[day][TicketType.AllDayRegular].Add(seat);
            }
            else if (type == TicketType.Deluxe || type == TicketType.AllDayDeluxe)
            {
                show.DailyBookedSeats[day][TicketType.Deluxe].Add(seat);
                show.DailyBookedSeats[day][TicketType.AllDayDeluxe].Add(seat);
            }
        }

        static void RemoveSeatBooking(Show show, TicketType type, string seat, DayOfWeek day)
        {
            if (type == TicketType.Regular || type == TicketType.AllDayRegular)
            {
                show.DailyBookedSeats[day][TicketType.Regular].Remove(seat);
                show.DailyBookedSeats[day][TicketType.AllDayRegular].Remove(seat);
            }
            else if (type == TicketType.Deluxe || type == TicketType.AllDayDeluxe)
            {
                show.DailyBookedSeats[day][TicketType.Deluxe].Remove(seat);
                show.DailyBookedSeats[day][TicketType.AllDayDeluxe].Remove(seat);
            }
        }
    }
}
