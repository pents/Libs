using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedConsoleOut
{
    public static class ConsoleOut
    {
        private static long? CountDown = null;
        private static long clock;
        /// <summary>
        /// Обычное сообщение белым цветом
        /// </summary>
        public static void print(string msg)
        {
            Console.Out.WriteLine(msg);
        }

        private static void printColor(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            print(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Сообщение зеленым цветом
        /// </summary>
        public static void printApproved(string msg)
        {
            printColor(msg, ConsoleColor.Green);
        }

        /// <summary>
        /// Сообщение красным цветом
        /// </summary>
        public static void printError(string msg)
        {
            printColor(msg, ConsoleColor.Red);
        }

        /// <summary>
        /// Сообщение желтым цветом
        /// </summary>
        public static void printWarning(string msg)
        {
            printColor(msg, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Отображение загрузки в окне консоли
        /// </summary>
        /// <param name="progress">текущий прогресс</param>
        /// <param name="total">Всего</param>
        public static void drawTextProgressBar(int progress, int total)
        {
            if (CountDown == null)
            {
                clock = DateTime.Now.Ticks;
                CountDown = 0;
            }
            else
            {
                CountDown = clock;
                clock = DateTime.Now.Ticks;
                CountDown = (clock - CountDown) * (total - progress);
            }
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.Write("[");     // start
            Console.CursorLeft = 32;
            Console.Write("]");     // end
            Console.CursorLeft = 1;

            float onechunck = 30.0f / total;


            //отрисовка
            int posit = 1;
            for (int i = 0; i < onechunck * progress; ++i)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = posit++;
                Console.Write(" ");

            }

            for (int i = posit; i <= 31; ++i)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = posit++;
                Console.Write(" ");
            }

            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(string.Format("{0}% ", progress == total - 1 ? 100 : (progress * 100) / total + 1));

            Console.Write(string.Format("  Осталось: {0} минут {1} секунд  ", new TimeSpan((long)CountDown).Minutes, new TimeSpan((long)CountDown).Seconds));

            Console.CursorVisible = true;

        }
    }
}
