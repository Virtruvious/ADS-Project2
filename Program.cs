using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace POSTFIX
{
    class Program
    {
        static void Main()
        { 
            Console.WriteLine("Expression Evaluator\n--------------------\nEnsure all elements are spaced! eg: ( 6 - 1 ) + -5 > 4\nT = true, F = false, & = AND, | = OR, ! = LOGICAL NOT");
            Console.Write("Expression: ");

            string input = (Console.ReadLine() ?? "").ToLower();
            Console.WriteLine();

            if (!Regex.IsMatch(input, @"^[a-z0-9()+\-*\/^<>!=&|. ]*$")) // If input contains invalid characters exit
            {
                Console.WriteLine("Invalid input! Only accepts 1-9 a-z, +-*/^, <>!=, &| and ().");
                return;
            }

            input = input.Insert(input.Length, " #");
            input = input.Replace(">=", "≥");
            input = input.Replace("<=", "≤");
            input = input.Replace("!=", "≠");
                
            List<string> infix = new(input.Split(" "));

            List<string> postfix = InfixToPostfix(infix);
            Console.WriteLine("POSTFIX: " + string.Join("", postfix));
            Console.WriteLine("Evaluation: " + EvaluatePostFix(postfix));
        }

        static List<string> InfixToPostfix(List<string> input)
        {
            MyStack stack = new(100);
            stack.Push("#");

            List<string> output = new List<string>();

            foreach (string a in input)
            {
                switch (a)
                {
                    case "#": // Empty stack
                        for (int i = stack.top; i >= 0; i--)
                        {
                            output.Add(stack.contents[i]);
                        }

                        break;
                    case "(": // Left parenthesis
                        stack.Push(a);
                        break;
                    case ")": // Right parenthesis
                        while (stack.Peek() != "(")
                        {
                            output.Add(stack.Pop());
                        }

                        stack.Pop();
                        break;
                    default: // Operands and Variables
                        if (int.TryParse(a, out int parsedValue) || Regex.IsMatch(a, @"^[a-z]*$"))
                        {
                            output.Add(a);
                        }
                        else // Operators
                        {
                            while (getISP(stack.Peek()) >= getICP(a))
                            {
                                output.Add(stack.Pop());
                            }

                            stack.Push(a);
                        }

                        break;
                }
            }

            static int getISP(string c) => c switch
            {
                "^" => 6,
                "*" or "/" => 5,
                "+" or "-" => 4,
                ">" or "<" or "≤" or "≥" or "=" or "≠" or "!" => 3,
                "&" => 2,
                "|" => 1,
                "(" => 0,
                "#" => -1,
            };

            static int getICP(string c) => c switch
            {
                "^" => 7,
                "*" or "/" => 5,
                "+" or "-" => 4,
                ">" or "<" or "≤" or "≥" or "=" or "≠" or "!" => 3,
                "&" => 2,
                "|" => 1,
                "(" => 7,
                "#" => -1
            };

            return output;
        }

        static string EvaluatePostFix(List<string> input)
        {
            input.Remove("#");
            //Stack<string> stack = new();
            MyStack stack = new(100);
            Dictionary<string, string> variables = new();

            foreach (string a in input)
            {
                if (Regex.IsMatch(a, @"^[a-z]*$")) // If letter, ask for value and store on stack
                {
                    if(variables.ContainsKey(a)) // If variable already stored in dictionary, use that value
                        stack.Push(variables[a]);
                    else if (a == "t" || a == "f") // If T or F, store on stack
                        stack.Push(a.ToUpper());
                    else // Else ask for value and store in dictionary
                    {
                        Console.Write($"Enter value for {a}: ");
                        string varInput = Console.ReadLine() ?? "";
                        variables.Add(a, varInput);
                        stack.Push(varInput);
                    }
                    continue;
                }

                if (int.TryParse(a, out int parsedValue)) // If number, store on stack
                {
                    stack.Push(a);
                }
                else // Else operator, pop two values and perform operation
                {
                    if (double.TryParse(stack.Peek(), out double num)) // Number Logic
                    {
                        stack.Pop();
                        double op2 = num;
                        double op1 = stack.contents.Length == 0 ? 0 : double.Parse(stack.Pop()); // Fail safe for negative values

                        switch (a)
                        {
                            case "+":
                                stack.Push((op1 + op2).ToString());
                                break;
                            case "-":
                                stack.Push((op1 - op2).ToString());
                                break;
                            case "*":
                                stack.Push((op1 * op2).ToString());
                                break;
                            case "/":
                                stack.Push((op1 / op2).ToString());
                                break;
                            case "^":
                                stack.Push((Math.Pow(op1, op2)).ToString());
                                break;
                            case ">":
                                stack.Push(op1 > op2 ? "T" : "F");
                                break;
                            case "<":
                                stack.Push(op1 < op2 ? "T" : "F");
                                break;
                            case "≥":
                                stack.Push(op1 >= op2 ? "T" : "F");
                                break;
                            case "≤":
                                stack.Push(op1 <= op2 ? "T" : "F");
                                break;
                            case "=":
                                stack.Push(op1 == op2 ? "T" : "F");
                                break;
                            case "≠":
                                stack.Push(op1 != op2 ? "T" : "F");
                                break;
                        }
                    }
                    else // Predicate Logic
                    {
                        string op2 = stack.Pop();
                        string op1 = "F";
                        if (a != "!")
                        {
                            op1 = stack.contents.Length == 0 ? "F" : stack.Pop(); // Fail safe for empty stack
                        }

                        switch (a)
                        {
                            case "&":
                                stack.Push(op1 == "T" && op2 == "T" ? "T" : "F");
                                break;
                            case "|":
                                stack.Push(op1 == "T" || op2 == "T" ? "T" : "F");
                                break;
                            case "≠":
                                stack.Push(op1 != op2 ? "T" : "F");
                                break;
                            case "!":
                                switch (op2)
                                {
                                    case "T":
                                        stack.Push("F");
                                        break;
                                    case "F":
                                        stack.Push("T");
                                        break;
                                }
                                break;
                        }
                    }
                }
            }

            switch (stack.Peek())
            {
                case "T":
                    stack.Pop();
                    return "True";
                case "F":
                    stack.Pop();
                    return "False";
                default:
                    return stack.Pop();
            }
        }
    }
}