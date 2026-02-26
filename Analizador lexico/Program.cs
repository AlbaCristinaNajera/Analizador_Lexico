using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

    public enum TipoToken
    {
        Identificador,
        Entero,
        Decimal,
        PalabraReservada,
        Operador,
        Simbolo,
        Cadena,
        Caracter,
        Desconocido
    }

    public class Token
    {
        public string Lexema { get; set; }
        public TipoToken Tipo { get; set; }

        public override string ToString() => $"{Lexema} → {Tipo}";
    }

    public class AnalizadorLexico
    {
        private static readonly HashSet<string> PalabrasReservadas = new HashSet<string>(StringComparer.Ordinal)
        {
            "int", "float", "if", "else", "while", "return", "for", "break", "continue", "bool", "string", "char", "double", "void"
        };

  
        private static readonly Regex TokenRegex = new Regex(
            @"
            #Cadena
            (""(?:\\.|[^""])*"") | 
            #Caracter
            ('(?:\\.|[^'])')       | 
             # Operadores multicaracter
            (==|!=|<=|>=|\+\+|--|&&|\|\|) | 
              # Identificador
            ([A-Za-z_]\w*)         | 
            # Número (decimal primero, luego entero)
            (\d+\.\d+|\d+)         |
            # Operador de un carácter
            ([+\-*/%=<>!&|^~])     | 
            # Símbolos
            ([;:,()\[\]{}\.])                      
            ",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

        public List<Token> Analizar(string codigo)
        {
            var tokens = new List<Token>();

            // Eliminar comentarios antes de analizar los tokens
            string sinComentarios = Regex.Replace(codigo, @"//.*?$|/\*.*?\*/", "", RegexOptions.Singleline | RegexOptions.Multiline);

     
            var matches = TokenRegex.Matches(sinComentarios);
            foreach (Match m in matches)
            {
                string lexema = m.Value;
                TipoToken tipo;

                if (string.IsNullOrWhiteSpace(lexema))
                    continue;

                if (lexema.StartsWith("\"") && lexema.EndsWith("\""))
                    tipo = TipoToken.Cadena;
                else if (lexema.StartsWith("'") && lexema.EndsWith("'"))
                    tipo = TipoToken.Caracter;
                else if (PalabrasReservadas.Contains(lexema))
                    tipo = TipoToken.PalabraReservada;
                else if (Regex.IsMatch(lexema, @"^\d+\.\d+$"))
                    tipo = TipoToken.Decimal;
                else if (Regex.IsMatch(lexema, @"^\d+$"))
                    tipo = TipoToken.Entero;
                else if (Regex.IsMatch(lexema, @"^[A-Za-z_]\w*$"))
                    tipo = TipoToken.Identificador;
                else if (Regex.IsMatch(lexema, @"^(==|!=|<=|>=|\+\+|--|&&|\|\||[+\-*/%=<>!&|^~])$"))
                    tipo = TipoToken.Operador;
                else if (Regex.IsMatch(lexema, @"^[;:,()\[\]{}\.]$"))
                    tipo = TipoToken.Simbolo;
                else
                    tipo = TipoToken.Desconocido;

                tokens.Add(new Token { Lexema = lexema, Tipo = tipo });
            }

            return tokens;
        }
    }

    class Program
    {
        public static object? Lexema { get; private set; }

        static void Main()
        {
            string codigo = @"
                // Comentario de una línea
                /* Comentario
                   multilínea */
                int x = 10;
                float y = 3.14;
                string s = ""hola \""mundo\"""";
            if (x >= 5 && y < 10.0)
            {
                x++;
                y = y + 1.0;
            }
            ";

            var analizador = new AnalizadorLexico();
            var tokens = analizador.Analizar(codigo);

            Console.WriteLine("Tabla Lexema        -->          Token:");
            Console.WriteLine(new string('-', 40));
            foreach (var token in tokens)
            {
         
                Console.WriteLine("{0,-25} --> {1}", token.Lexema, token.Tipo);
            }
        }
    }