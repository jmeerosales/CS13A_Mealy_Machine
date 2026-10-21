using System;
using System.Text;

class MealyMachineComment
{
    // Made public so Transition() can return it without CS0050 error
    public enum State { q0, q1, q2, q3, q4, qtrap }

    /// <summary>
    /// Mealy Machine Transition Function δ(currentState, symbol) -> (nextState, output)
    /// Alphabet: Σ = { '/', '*', 'a' (any other character) }
    /// Output Alphabet: Λ = { '0', '1' }
    /// </summary>
    public static (State nextState, char output) Transition(State currentState, char symbol)
    {
        switch (currentState)
        {
            case State.q0:
                // δ(q0, /) = q1/0 ; δ(q0, *) = qtrap/0 ; δ(q0, a) = qtrap/0
                if (symbol == '/') return (State.q1, '0');
                else return (State.qtrap, '0');

            case State.q1:
                // Both '//' (single-line) and '/*' (multi-line) enter comment state q2
                // δ(q1, /) = q2/0 ; δ(q1, *) = q2/0 ; δ(q1, a) = qtrap/0
                if (symbol == '/' || symbol == '*') return (State.q2, '0');
                else return (State.qtrap, '0');

            case State.q2:
                // Inside comment body
                // δ(q2, *) = q3/0 ; δ(q2, /) = q2/0 ; δ(q2, a) = q2/0
                if (symbol == '*') return (State.q3, '0');
                else return (State.q2, '0');

            case State.q3:
                // Saw '*' inside comment body watching for '/'
                // δ(q3, /) = q4/1 ; δ(q3, *) = q3/0 ; δ(q3, a) = q2/0
                if (symbol == '/') return (State.q4, '1');
                else if (symbol == '*') return (State.q3, '0');
                else return (State.q2, '0');

            case State.q4:
                // Comment already closed. Any trailing characters go to qtrap
                // δ(q4, symbol) = qtrap/0
                return (State.qtrap, '0');

            case State.qtrap:
            default:
                // Trap state loops on all symbols
                // δ(qtrap, symbol) = qtrap/0
                return (State.qtrap, '0');
        }
    }

    public static bool Evaluate(string input, out string stateTrace, out string outputStream)
    {
        State currentState = State.q0;
        StringBuilder traceBuilder = new StringBuilder();
        StringBuilder outputBuilder = new StringBuilder();

        traceBuilder.Append(currentState);

        foreach (char symbol in input)
        {
            var (nextState, output) = Transition(currentState, symbol);
            
            currentState = nextState;
            traceBuilder.Append(" -> " + currentState);
            outputBuilder.Append(output);
        }

        stateTrace = traceBuilder.ToString();
        outputStream = outputBuilder.ToString();

        // Accepted if final state is q4 (ends with output '1')
        return currentState == State.q4;
    }

    static void Main()
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("     Mealy Machine C-Style Comment Evaluator     ");
        Console.WriteLine("       Σ = {/, *, a} | Output Λ = {0, 1}          ");
        Console.WriteLine("==================================================");
        Console.WriteLine("Type 'exit' to quit.\n");

        while (true)
        {
            Console.Write("Enter string to test: ");
            string input = Console.ReadLine();

            if (input == null) break;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exiting program...");
                break;
            }

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Result: REJECTED (Empty input)\n");
                continue;
            }

            bool isAccepted = Evaluate(input, out string trace, out string outputs);

            Console.WriteLine($"State Trace  : {trace}");
            Console.WriteLine($"Outputs      : {outputs}");

            if (isAccepted)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Result       : ACCEPTED\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Result       : REJECTED\n");
            }

            Console.ResetColor();
        }
    }
}
