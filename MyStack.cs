using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSTFIX
{
    internal class MyStack
    {
        public int top;
        public string[] contents;
        private int size;

        public MyStack(int size)
        {
            this.size = size;
            contents = new string[size];
            top = -1;
        }

        public void Push(string item)
        {
            if (top == size - 1)
            {
                Console.WriteLine("Stack is full!");
            }
            else
            {
                top++;
                contents[top] = item;
            }
        }

        public string Pop()
        {
            if (top == -1)
            {
                Console.WriteLine("Stack is empty!");
                return "0";
            }
            else
            {
                string item = contents[top];
                top--;
                return item;
            }
        }

        public string Peek()
        {
            if (top == -1)
            {
                Console.WriteLine("Stack is empty!");
                return "0";
            }
            else
            {
                return contents[top];
            }
        }


    }
}
