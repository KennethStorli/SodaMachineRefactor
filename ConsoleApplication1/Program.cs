using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SodaMachine sodaMachine = new SodaMachine();
            sodaMachine.Start();
        }
    }

    public class SodaMachine
    {
        private static int money;

        private List<Soda> Inventory { get; set; } = new List<Soda>();

        public enum CommandType
        {
            UnrecognizedCommand = -99,
            Insert = 0,
            Order = 1,
            OrderBySms = 2,
            Recall = 3
        }

        public SodaMachine()
        {
            Inventory.Add(new Soda { Name = "coke", Nr = 5, Price = 20 });
            Inventory.Add(new Soda { Name = "sprite", Nr = 3, Price = 15 });
            Inventory.Add(new Soda { Name = "fanta", Nr = 3, Price = 15 });
        }

        /// <summary>
        /// This is the starter method for the machine
        /// </summary>
        public void Start()
        {
            while (true)
            {
                PrintInstructions();

                var input = Console.ReadLine();

                var commandType = ParseCommand(input);

                switch (commandType)
                {
                    case CommandType.Insert:
                        //Add to credit
                        int insertedMoney;
                        bool parseSuccess = int.TryParse(input.Split(' ')[1], out insertedMoney);
                        if (parseSuccess && insertedMoney > 0)
                        {
                            money += insertedMoney;
                            Console.WriteLine($"Adding {insertedMoney} to credit");
                        }
                        else
                        {
                            Console.WriteLine("Please only insert valid money into the soda machine!");
                        }
                        break;
                    case CommandType.Order:
                    case CommandType.OrderBySms:
                        //Attempt to find the desired soda
                        var soda = FindSodaType(commandType, input);
                        if (soda == null)
                        {
                            //If no such soda is found, print message and continue to next loop iteration
                            Console.WriteLine("No such soda");
                            continue;
                        }
                        else
                        {
                            //if soda is found, attempt to buy it
                            this.BuySoda(soda, commandType);
                        }
                        break;
                    case CommandType.Recall:
                        //Give money back
                        Console.WriteLine($"Returning {money} to customer");
                        money = 0;
                        break;
                    default:

                        break;
                }

            }
        }

        /// <summary>
        /// This method prints the instructions to the user.
        /// </summary>
        public void PrintInstructions()
        {
            Console.WriteLine("\n\nAvailable commands:");
            Console.WriteLine("insert (money) - Money put into money slot");
            Console.WriteLine("order (coke, sprite, fanta) - Order from machines buttons");
            Console.WriteLine("sms order (coke, sprite, fanta) - Order sent by sms");
            Console.WriteLine("recall - gives money back");
            Console.WriteLine("-------");
            Console.WriteLine("Inserted money: " + money);
            Console.WriteLine("-------\n\n");
        }

        /// <summary>
        /// This method takes the input string that comes from the user and the command type of that input string, 
        /// and uses this to find the soda the user is attempting to order based on what command they entered.
        /// If the command is neither an Order nor an OrderBySms, or if the desired soda is not found, null is returned.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public Soda FindSodaType(CommandType cmd, string input)
        {
            string sodaString = "";
            if (cmd == CommandType.Order)
            {
                sodaString = input.Split(' ')[1];
            }
            else if(cmd == CommandType.OrderBySms)
            {
                sodaString = input.Split(' ')[2];
            }

            return Inventory.Where(inv => inv.Name == sodaString).FirstOrDefault(); ;
        }

        /// <summary>
        /// This method takes an input string and attempts to parse it into a command recognized by the system.
        /// It returns a CommandType that describes what command it was given.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public CommandType ParseCommand(string input)
        {
            if (input.StartsWith("insert"))
            {
                return CommandType.Insert;
            }
            else if (input.StartsWith("order"))
            {
                return CommandType.Order;
            }
            else if (input.StartsWith("sms order"))
            {
                return CommandType.OrderBySms;
            }
            else if (input.Equals("recall"))
            {
                return CommandType.Recall;
            }
            else
            {
                return CommandType.UnrecognizedCommand;
            }
        }

        /// <summary>
        /// This method takes in a soda type and a command type, 
        /// and attempts to buy the soda based on what command it is given.
        /// </summary>
        /// <param name="soda"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool BuySoda(Soda soda, CommandType cmd)
        {
            if(soda.Nr <= 0)
            {
                Console.WriteLine($"No {soda.Name} left");
                return false;
            }

            if(cmd == CommandType.Order)
            {
                if(money >= soda.Price)
                {
                    Console.WriteLine($"Giving {soda.Name} out");
                    money -= soda.Price;
                    Console.WriteLine($"Giving {money} out in change");
                    money = 0;
                    soda.Nr--;
                    return true;
                }
                else
                {
                    Console.WriteLine("Need " + (soda.Price - money) + " more");
                }
            }
            else if(cmd == CommandType.OrderBySms)
            {
                Console.WriteLine($"Giving {soda.Name} out");
                soda.Nr--;
                return true;
            }
            
            return false;
        }

    }
    public class Soda
    {
        public string Name { get; set; }
        public int Nr { get; set; }
        public int Price { get; set; }

    }
}
