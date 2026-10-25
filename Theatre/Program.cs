using System.Globalization;
using System.Text;

namespace Theatre
{
    internal class Program
    {
        // Місткість сховища
        const int Capacity = 100;

        // Вісім паралельних масивів: елементи з індексом i разом описують одну виставу
        static int[] ids = new int[Capacity];               // код вистави
        static string[] titles = new string[Capacity];      // назва
        static string[] genres = new string[Capacity];      // жанр
        static string[] directors = new string[Capacity];   // режисер
        static DateTime[] dates = new DateTime[Capacity];   // дата й час
        static int[] durations = new int[Capacity];         // тривалість, хвилин
        static decimal[] prices = new decimal[Capacity];    // ціна квитка, грн
        static int[] sold = new int[Capacity];              // продано квитків

        static int count = 0;   // скільки перших комірок реально заповнено
        static int nextId = 1;  // лічильник для автонумерації вистав

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Seed(); // демонстраційні дані

            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("=== Театр: облік вистав ===");
                Console.WriteLine("1 — додати виставу");
                Console.WriteLine("2 — показати всі вистави");
                Console.WriteLine("3 — знайти виставу за кодом");
                Console.WriteLine("4 — видалити виставу за кодом");
                Console.WriteLine("0 — вихід");
                Console.Write("Ваш вибір: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                switch (choice)
                {
                    case "1": AddShow(); break;
                    case "2": PrintAll(); break;
                    case "3": FindById(); break;
                    case "4": DeleteById(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Немає такого пункту меню."); break;
                }
            }

            Console.WriteLine("Роботу завершено.");
        }

        //Запис у першу вільну комірку всіх масивів
        static void Store(string title, string genre, string director,
                          DateTime date, int duration, decimal price, int soldTickets)
        {
            ids[count] = nextId;
            titles[count] = title;
            genres[count] = genre;
            directors[count] = director;
            dates[count] = date;
            durations[count] = duration;
            prices[count] = price;
            sold[count] = soldTickets;

            count++;   // зайнятих комірок стало на одну більше
            nextId++;  // наступна вистава отримає інший код
        }

        // Демонстраційні дані
        static void Seed()
        {
            Store("Лісова пісня", "драма", "Іваненко Т.", new DateTime(2026, 11, 7, 18, 0, 0), 150, 350m, 180);
            Store("Майська ніч", "комедія", "Петренко О.", new DateTime(2026, 11, 14, 19, 0, 0), 120, 280m, 150);
            Store("Тіні забутих предків", "драма", "Мельник В.", new DateTime(2026, 11, 21, 18, 30, 0), 135, 400m, 95);
        }

        //  Пошук позиції вистави за кодом: індекс або -1 
        static int IndexOfId(int id)
        {
            for (int i = 0; i < count; i++) // тільки до count, а не до Capacity
            {
                if (ids[i] == id) return i;
            }
            return -1;
        }

        //Дія 1: додати виставу
        static void AddShow()
        {
            if (count >= Capacity)
            {
                Console.WriteLine($"Сховище заповнене: більше за {Capacity} вистав не вміщується.");
                return;
            }

            Console.WriteLine("--- Нова вистава ---");

            Console.Write("Назва: ");
            string title = (Console.ReadLine() ?? "").Trim();
            if (title.Length == 0)
            {
                Console.WriteLine("Назва не може бути порожньою. Виставу не додано.");
                return;
            }

            Console.Write("Жанр: ");
            string genre = (Console.ReadLine() ?? "").Trim();
            if (genre.Length == 0)
            {
                Console.WriteLine("Жанр не може бути порожнім. Виставу не додано.");
                return;
            }

            Console.Write("Режисер: ");
            string director = (Console.ReadLine() ?? "").Trim();
            if (director.Length == 0)
            {
                Console.WriteLine("Режисер не може бути порожнім. Виставу не додано.");
                return;
            }

            Console.Write("Дата й час (дд.мм.рррр гг:хх): ");
            bool dateOk = DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None, out DateTime date);
            if (!dateOk)
            {
                Console.WriteLine("Дату не розпізнано. Потрібен формат 07.11.2026 18:00. Виставу не додано.");
                return;
            }

            Console.Write("Тривалість, хвилин: ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
            {
                Console.WriteLine("Тривалість має бути додатним цілим числом. Виставу не додано.");
                return;
            }

            Console.Write("Ціна квитка, грн: ");
            // Кому замінюємо на крапку, щоб приймати і 350,50, і 350.50
            string priceText = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
            bool priceOk = decimal.TryParse(priceText, NumberStyles.Number,
                                            CultureInfo.InvariantCulture, out decimal price);
            if (!priceOk || price < 0)
            {
                Console.WriteLine("Ціна має бути невід'ємним числом. Виставу не додано.");
                return;
            }

            Console.Write("Продано квитків: ");
            if (!int.TryParse(Console.ReadLine(), out int soldTickets) || soldTickets < 0)
            {
                Console.WriteLine("Кількість має бути цілим невід'ємним числом. Виставу не додано.");
                return;
            }

            Store(title, genre, director, date, duration, price, soldTickets);
            Console.WriteLine($"Додано виставу №{ids[count - 1]} у позицію {count - 1}. " +
                              $"Зайнято комірок: {count} зі {Capacity}.");
        }

        // Дія 2: показати всі вистави
        static void PrintAll()
        {
            if (count == 0)
            {
                Console.WriteLine("Вистав поки немає.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  № | Назва                | Жанр       | Режисер      | Дата й час       |   Хв |     Ціна | Продано |     Виторг");
            Console.WriteLine("----+----------------------+------------+--------------+------------------+------+----------+---------+-----------");

            int totalSold = 0;
            decimal totalRevenue = 0;
            int bestIndex = 0;
            decimal bestRevenue = prices[0] * sold[0]; // початкове значення — перший елемент

            for (int i = 0; i < count; i++)
            {
                decimal revenue = prices[i] * sold[i]; // виторг не зберігаємо, а рахуємо
                Console.WriteLine($"{ids[i],3} | {titles[i],-20} | {genres[i],-10} | {directors[i],-12} | " +
                                  $"{dates[i]:dd.MM.yyyy HH:mm} | {durations[i],4} | " +
                                  $"{prices[i],8:F2} | {sold[i],7} | {revenue,10:F2}");

                totalSold += sold[i];
                totalRevenue += revenue;

                if (revenue > bestRevenue)
                {
                    bestRevenue = revenue;
                    bestIndex = i;
                }
            }

            Console.WriteLine($"Усього вистав: {count}, продано квитків: {totalSold}, " +
                              $"касовий збір: {totalRevenue:F2} грн.");
            Console.WriteLine($"Найкасовіша вистава: «{titles[bestIndex]}» — {bestRevenue:F2} грн.");
        }

        //Дія 3: знайти виставу за кодом
        static void FindById()
        {
            Console.Write("Код вистави для пошуку: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Код має бути цілим числом.");
                return;
            }

            int index = IndexOfId(id);
            if (index == -1)
            {
                Console.WriteLine($"Вистави №{id} у списку немає.");
                return;
            }

            Console.WriteLine($"Знайдено в позиції {index}:");
            Console.WriteLine($"  назва:       {titles[index]}");
            Console.WriteLine($"  жанр:        {genres[index]}");
            Console.WriteLine($"  режисер:     {directors[index]}");
            Console.WriteLine($"  дата й час:  {dates[index]:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"  тривалість:  {durations[index]} хв");
            Console.WriteLine($"  ціна квитка: {prices[index]:F2} грн");
            Console.WriteLine($"  продано:     {sold[index]} квитків");
            Console.WriteLine($"  виторг:      {prices[index] * sold[index]:F2} грн");
        }

        // Дія 4: видалити виставу за кодом
        static void DeleteById()
        {
            Console.Write("Код вистави для видалення: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Код має бути цілим числом.");
                return;
            }

            int index = IndexOfId(id);
            if (index == -1)
            {
                Console.WriteLine($"Вистави №{id} у списку немає.");
                return;
            }

            Console.WriteLine($"Видалено виставу №{ids[index]} ({titles[index]}, {directors[index]}).");

            // Зсув уліво одночасно в усіх вісьмох масивах, інакше поля переплутаються
            for (int i = index; i < count - 1; i++)
            {
                ids[i] = ids[i + 1];
                titles[i] = titles[i + 1];
                genres[i] = genres[i + 1];
                directors[i] = directors[i + 1];
                dates[i] = dates[i + 1];
                durations[i] = durations[i + 1];
                prices[i] = prices[i + 1];
                sold[i] = sold[i + 1];
            }

            count--; // остання комірка більше не входить у робочу частину

            // Прибираємо «хвіст», щоб у вивільненій комірці не лишалося старих даних
            ids[count] = 0;
            titles[count] = "";
            genres[count] = "";
            directors[count] = "";
            dates[count] = default;
            durations[count] = 0;
            prices[count] = 0m;
            sold[count] = 0;

            Console.WriteLine($"Зайнято комірок: {count} зі {Capacity}.");
        }
    }
}

