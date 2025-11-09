using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
        public Token()
        {
            // Default constructor
        }
           
        public Token(string lex, Token_Class token_type)
        {
            this.lex = lex;
            this.token_type = token_type;
        }
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INTEGER", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i + 1;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (char.IsLetter(CurrentChar)) //if you read a character
                {
                    while (j < SourceCode.Length && char.IsLetter(SourceCode[j]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }

                else if(char.IsDigit(CurrentChar))
                {
                    while (j < SourceCode.Length && char.IsDigit(SourceCode[j]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    
                    if (j < SourceCode.Length && SourceCode[j] == '.' && j + 1 < SourceCode.Length && char.IsDigit(SourceCode[j + 1]))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;

                        while (j < SourceCode.Length && char.IsDigit(SourceCode[j]))
                        { 
                            CurrentLexeme += SourceCode[j]; 
                            j++;
                        }
                        
                    }
                    
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else if(CurrentChar == '{' && j+1 < SourceCode.Length)
                {
                    j = i + 1;
                    while (j < SourceCode.Length && SourceCode[j] != '}')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    if (j < SourceCode.Length && SourceCode[j] == '}')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    else
                    {
                        Errors.Error_List.Add("Unterminated comment");
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
                else
                {
                    while (j < SourceCode.Length &&
                           !char.IsLetter(SourceCode[j]) &&
                           !char.IsDigit(SourceCode[j]) &&
                           !char.IsWhiteSpace(SourceCode[j]) &&
                           SourceCode[j] != '{' &&
                           SourceCode[j] != '}')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex.ToUpper()))
            {
                Tok.token_type = ReservedWords[Lex.ToUpper()];
                Tokens.Add(Tok);
                return;
            }
            else if (isIdentifier(Lex)) //Is it an identifier?
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                return;
            }            
            else if (Operators.ContainsKey((Lex))) //Is it an operator?
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
                return;
            }
            else if (isConstant(Lex)) //Is it a Constant? 
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                return;
            }

            else //Is it an undefined?
            {
                Errors.Error_List.Add($"Unknown token: {Lex}");
            }
        }
        
        // Check if the lex is an identifier or not.
        bool isIdentifier(string lex)
        {
            bool isValid=true;

            if (string.IsNullOrEmpty(lex) || !char.IsLetter(lex[0]))
            {
                return false;
            }

            for (int i = 1; i < lex.Length; i++)
            {
                if (!char.IsLetterOrDigit(lex[i]) && lex[i] != '_')
                    return false;
            }
            
            return isValid;
        }
        // Check if the lex is a constant (Number) or not.
        bool isConstant(string lex)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(lex)) return false;

            int dotCount = 0;

            foreach (char c in lex)
            {
                if (c == '.')
                {
                    dotCount++;
                    if (dotCount > 1)
                        return false;
                }
                else if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return isValid;
        }
    }
}
